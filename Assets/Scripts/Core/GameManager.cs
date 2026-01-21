using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public int maxLives = 3;
    public int currentLives;

    [Header("Coins")]
    public int coins;

    [Header("Pause")]
    public bool isPaused;
    public GameObject pausePanel; // 🔹 ARRASTE O PAUSE PANEL AQUI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetGame();

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    // ======================
    // GAME FLOW
    // ======================

    public void ResetGame()
    {
        currentLives = maxLives;
        coins = 0;
        ResumeGame();
    }

    // ======================
    // PAUSE
    // ======================

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // ======================
    // UI BUTTONS
    // ======================

    public void ResumeButton()
    {
        ResumeGame();
    }

    public void LoadMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene("Level_01");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    internal void AddCoin(int value)
    {
        throw new NotImplementedException();
    }
}
