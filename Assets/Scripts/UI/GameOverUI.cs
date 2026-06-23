using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string resumeScene = "LoginFake";
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(resumeScene);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Options()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
}
