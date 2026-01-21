using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public float waitTime = 5f;
    public string mainMenuScene = "MainMenu";

    private void Start()
    {
        Time.timeScale = 1f; // garante que o tempo esteja rodando
        Invoke(nameof(GoToMainMenu), waitTime);
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}


