using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string playSceneName = "GameScene";

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject leaderboardPanel;
    public GameObject helpPanel;

    public void OnPlayButton()
    {
        SceneManager.LoadScene(playSceneName);
    }

    public void OnButtonClickChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnButtonClickChangeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void OnOptionsButton()
    {
        ShowPanel(optionsPanel);
    }

    public void OnLeaderboardButton()
    {
        ShowPanel(leaderboardPanel);
    }

    public void OnHelpButton()
    {
        ShowPanel(helpPanel);
    }

    public void OnClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        // Hide all panels first
        optionsPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        helpPanel.SetActive(false);

        // Show the requested panel
        panel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
