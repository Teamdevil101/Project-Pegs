using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names or Indexes")]
    public string playSceneName = "GameScene"; // Replace with your game scene name

    // Functions to assign to buttons
    public void OnPlayButton()
    {
        SceneManager.LoadScene(playSceneName);
    }

    public void OnOptionsButton()
    {
        Debug.Log("Options button clicked!");
        // Here you can open your Options menu panel
    }

    public void OnLeaderboardButton()
    {
        Debug.Log("Leaderboard button clicked!");
        // Here you can open your Leaderboard panel
    }

    public void OnHelpButton()
    {
        Debug.Log("Help button clicked!");
        // Here you can open your Help panel
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button clicked!");
        Application.Quit();
    }
}

