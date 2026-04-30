using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    [Header("Flow")]
    [SerializeField] private string firstSceneAfterMenu = "LoginFake";

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstSceneAfterMenu);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

