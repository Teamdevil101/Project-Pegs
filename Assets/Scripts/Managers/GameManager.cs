using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform pegContainer;

    public TMP_Text pegCounterElement; // FOR TESTING!

    private List<GameObject> allPegs = new();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform peg in pegContainer)
        {
            allPegs.Add(peg.gameObject);
        }

        SetRandomOrangePegs(25);
    }

    // Update is called once per frame
    void Update()
    {
        pegCounterElement.text = $"Peg Counter:\nTotal: {GetAllPegsCount(false)}\nOrange Total: {GetAllOrangePegsCount(false)}" +
            $"\nTotal Left: {GetAllPegsCount()}\nOrange Left: {GetAllOrangePegsCount()}"; // Should be moved out of here.
    }

    private void SetRandomOrangePegs(int totalCount)
    {
        if(allPegs.Count < totalCount)
            totalCount = allPegs.Count;

        for (int i = 0; i < totalCount; i++)
        {
            int rngPegIndex = Random.Range(0, allPegs.Count);
            
            while (allPegs[rngPegIndex].CompareTag("OrangePeg")) // Still a placeholder way to find OrangePegs...
            {
                rngPegIndex = (rngPegIndex + 1) % allPegs.Count;
            }

            allPegs[rngPegIndex].tag = "OrangePeg";
            if (allPegs[rngPegIndex].TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = new Color(1f, 0.3f, 0f, 1f);
            }
        }
    }

    public void ChangeState(GameState newState) => currentGameState = newState;
    public GameState GetCurrentState => currentGameState;

    public List<GameObject> GetAllOrangePegs(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.CompareTag("OrangePeg") && g.activeSelf).ToList(); // Placeholder way to find orange pegs
        else
            return allPegs.Where(g => g.CompareTag("OrangePeg")).ToList(); // Placeholder way to find orange pegs
    }

    public List<GameObject> GetAllPegs(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.activeSelf).ToList();
        else
            return allPegs;
    }

    public int GetAllPegsCount(bool excludeVanished = true)
    {
        if(excludeVanished)
            return allPegs.Where(g => g.activeSelf).Count();
        else
            return allPegs.Count;
    }

    public int GetAllOrangePegsCount(bool excludeVanished = true)
    {
        if (excludeVanished)
            return allPegs.Where(g => g.CompareTag("OrangePeg") && g.activeSelf).Count();
        else
            return allPegs.Where(g => g.CompareTag("OrangePeg")).Count();
    }

    public enum GameState
    {
        Start,
        Aim,
        Shot,
        Final
    }
}
