using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuUI : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
