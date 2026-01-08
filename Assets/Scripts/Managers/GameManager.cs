using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform pegContainer;
    public short startingBallCount;
    public int startingOrangePegCount;

    public TMP_Text pegCounterElement; // FOR TESTING!

    public float pegDisableDelay = 0.1f;

    [Space]
    public PegData[] pegData;

    private float pegTimer = 0f;
    private int pegIndex = 0;
    private PegAction currentSpecialPeg = null;

    private short totalBallCount;
    private short totalActiveBallCount;

    private List<PegAction> allPegs = new();
    private List<PegAction> pegsToDisable = new();
    private GameState currentGameState = GameState.Start;

    public bool isACustomLevel = false;
    public static LevelData customLevelData;
    public static string customLevelPath;

    [SerializeField] private LevelObjectRegistry registry;
    [SerializeField] private string levelEditorSceneName = "LevelEditor";
    [SerializeField] private string[] pegTypeIds = { "peg", "squarepeg" };
    private HashSet<string> pegTypeIdSet;
    [SerializeField] private Transform nonPegContainer;

    public int GetTotalHitPegCount => pegsToDisable.Count;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError($"Multiple GameManagers detected! " +
                $"You can't have more than one! Deleting the extra manager in {gameObject.name}");
            Destroy(gameObject);
        }

        pegTypeIdSet = new HashSet<string>(pegTypeIds);

        if (isACustomLevel)
        {
            LoadLevel(customLevelData);
        }
    }

    void Start()
    {
        totalBallCount = startingBallCount;

        foreach (Transform peg in pegContainer)
        {
            allPegs.Add(peg.GetComponent<PegAction>());
        }

        SetRandomOrangePegs(startingOrangePegCount);
        SetRandomPurplePeg();
        currentGameState = GameState.Aim;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isACustomLevel)
                SceneManager.LoadScene("LevelEditor");
            else
                SceneManager.LoadScene(0);
        }
    }

    void FixedUpdate()
    {
        if(currentGameState == GameState.Shot)
        {
            if(totalActiveBallCount == 0)
            {
                if (pegsToDisable.Count == 0)
                {
                    currentGameState = GameState.Aim;
                    SetRandomPurplePeg();
                    SoundManager.instance.PlaySound(SoundManager.instance.reloadedSound);
                }

                if (pegIndex >= pegsToDisable.Count) return;

                pegTimer += Time.deltaTime;
                if (pegTimer >= pegDisableDelay)
                {
                    SoundManager.instance.PlaySound(pegsToDisable[pegIndex].pegPopSound);
                    pegsToDisable[pegIndex].gameObject.SetActive(false);
                    pegIndex++;
                    pegTimer = 0f;
                }

                if (pegIndex >= pegsToDisable.Count)
                {
                    pegIndex = 0;
                    pegsToDisable.Clear();
                }
            }
        }

        pegCounterElement.text = $"Peg Counter:\nTotal: {GetAllPegsCount(false)}\nOrange Total: {GetAllOrangePegsCount(false)}" +
            $"\nTotal Left: {GetAllPegsCount()}\nOrange Left: {GetAllOrangePegsCount()}" +
            $"\nBall Count: {totalBallCount}";// Should be moved out of here.
    }

    private void SetRandomOrangePegs(int totalCount)
    {
        if (allPegs == null || allPegs.Count == 0)
            return;

        var candidates = new List<PegAction>(allPegs);

        int count = Mathf.Min(totalCount, candidates.Count);

        Shuffle(candidates);

        for (int i = 0; i < count; i++)
        {
            candidates[i].SetPegType(PegAction.PegType.Mandatory);
            candidates[i].UpdatePegColor();
        }
    }

    private void SetRandomPurplePeg()
    {
        if (allPegs == null || allPegs.Count == 0)
            return;
            
        if (currentSpecialPeg != null)
        {
            currentSpecialPeg.SetPegType(PegAction.PegType.Regular);
            currentSpecialPeg.UpdatePegColor();
        }

        var candidates = new List<PegAction>();

        candidates.AddRange(allPegs.FindAll(p => p.MyPegType == PegAction.PegType.Regular));
        
        if (candidates.Count == 0)
        {
            Debug.Log("No valid pegs for purple selection. Skipping.");
            return;
        }

        int index = Random.Range(0, candidates.Count);

        currentSpecialPeg = candidates[index];
        currentSpecialPeg.SetPegType(PegAction.PegType.Special);
        currentSpecialPeg.UpdatePegColor();
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void ChangeState(GameState newState) => currentGameState = newState;
    public GameState GetCurrentState() => currentGameState;

    public List<PegAction> GetAllOrangePegs(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.MyPegType == PegAction.PegType.Mandatory && g.gameObject.activeSelf).ToList(); // Placeholder way to find orange pegs
        else
            return allPegs.Where(g => g.MyPegType == PegAction.PegType.Mandatory).ToList(); // Placeholder way to find orange pegs
    }

    public List<PegAction> GetAllPegs(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.gameObject.activeSelf).ToList();
        else
            return allPegs;
    }

    public int GetAllPegsCount(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.gameObject.activeSelf).Count();
        else
            return allPegs.Count;
    }

    public int GetAllOrangePegsCount(bool excludeVanished = true)
    {
        if (excludeVanished)
            return allPegs.Where(g => g.MyPegType == PegAction.PegType.Mandatory && g.gameObject.activeSelf).Count();
        else
            return allPegs.Where(g => g.MyPegType == PegAction.PegType.Mandatory).Count();
    }

    public void StoreForDestruction(GameObject obj) => pegsToDisable.Add(obj.GetComponent<PegAction>());

    public short GetTotalBallCount() => totalBallCount;
    public short GetTotalActiveBallCount() => totalActiveBallCount;

    public void AdjustBallCount(short amount)
    {
        totalBallCount += amount;

        if (totalBallCount < 0)
            totalBallCount = 0;
    }

    public void AdjustActiveBallToCount(short amount)
    {
        totalActiveBallCount += amount;

        if (totalActiveBallCount < 0)
            totalActiveBallCount = 0;
    }

    private void LoadLevel(LevelData levelData)
    {
        if (levelData == null) return;

        if (registry == null)
        {
            Debug.LogError("GameManager: registry is not assigned.");
            return;
        }

        if (pegTypeIdSet == null)
            pegTypeIdSet = new HashSet<string>(pegTypeIds);

        var pegRoot = pegContainer != null ? pegContainer : transform;
        var otherRoot = nonPegContainer != null ? nonPegContainer : transform;

        foreach (var objData in levelData.objects)
        {
            if (objData == null || string.IsNullOrEmpty(objData.objectTypeId))
                continue;

            var prefabs = registry.GetAllPrefabs(objData.objectTypeId);
            var prefab = prefabs != null ? prefabs.inGamePrefab : null;
            if (prefab == null)
            {
                Debug.LogWarning($"No inGamePrefab for '{objData.objectTypeId}'.");
                continue;
            }

            var parent = pegTypeIdSet.Contains(objData.objectTypeId) ? pegRoot : otherRoot;

            var rot = Quaternion.Euler(0f, 0f, objData.rotationZ);
            var instance = Instantiate(prefab, objData.position, rot, parent);
            instance.transform.localScale = objData.scale;

            if (!string.IsNullOrEmpty(objData.physicsMaterialId))
            {
                var mat = registry.GetMaterial(objData.physicsMaterialId);
                if (mat != null && instance.TryGetComponent<Collider2D>(out var col2D))
                    col2D.sharedMaterial = mat;
            }
        }
    }

    public enum GameState
    {
        Start,
        Aim,
        Shot,
        Final
    }

    [System.Serializable]
    public class PegData
    {
        public PegAction.PegType linkedType;
        public long basePoints;
        public Color baseColor = Color.white;
        public AudioClip impactSound;
    }
}
