using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuUI : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // Garante que a cena de Game Over não fique pausada.
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
