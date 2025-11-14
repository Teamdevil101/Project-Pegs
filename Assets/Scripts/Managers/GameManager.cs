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

    private float pegTimer = 0f;
    private int pegIndex = 0;
    private PegAction currentSpecialPeg = null;

    [Space]
    public PegData[] pegData;

    private short totalBallCount;
    private short totalActiveBallCount;

    private List<PegAction> allPegs = new();
    private List<PegAction> pegsToDisable = new();
    private GameState currentGameState = GameState.Start;

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
                }

                if (pegIndex >= pegsToDisable.Count) return;

                pegTimer += Time.deltaTime;
                if (pegTimer >= pegDisableDelay)
                {
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
        if(allPegs.Count < totalCount)
            totalCount = allPegs.Count;

        for (int i = 0; i < totalCount; i++)
        {
            int rngPegIndex = Random.Range(0, allPegs.Count);
            
            while (allPegs[rngPegIndex].MyPegType == PegAction.PegType.Mandatory) // Still a placeholder way to find OrangePegs...
            {
                rngPegIndex = (rngPegIndex + 1) % allPegs.Count;
            }

            allPegs[rngPegIndex].SetPegType(PegAction.PegType.Mandatory);
            allPegs[rngPegIndex].UpdatePegColor();
        }
    }

    private void SetRandomPurplePeg()
    {
        if (currentSpecialPeg != null)
        {
            currentSpecialPeg.SetPegType(PegAction.PegType.Regular);
            currentSpecialPeg.UpdatePegColor();
        }

        int rngPegIndex = Random.Range(0, allPegs.Count);

        while (allPegs[rngPegIndex].MyPegType == PegAction.PegType.Mandatory
                || allPegs[rngPegIndex].MyPegType == PegAction.PegType.PowerUp
                || !allPegs[rngPegIndex].gameObject.activeSelf)
        {
            rngPegIndex = (rngPegIndex + 1) % allPegs.Count;
        }

        currentSpecialPeg = allPegs[rngPegIndex];
        currentSpecialPeg.SetPegType(PegAction.PegType.Special);
        currentSpecialPeg.UpdatePegColor();
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
    }
}
