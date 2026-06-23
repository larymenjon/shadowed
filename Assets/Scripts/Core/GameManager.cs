using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int currentLives;

    [Header("Coins")]
    [SerializeField] private int coins;

    [Header("Pause")]
    [SerializeField] private bool isPaused;
    [SerializeField] private GameObject pausePanel;

    [Header("Level Flow")]
    [SerializeField] private string nextLevel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string restartSceneName = "LoginFake";

    private bool initialized;

    public int MaxLives => maxLives;
    public int CurrentLives => currentLives;
    public int Coins => coins;
    public bool IsPaused => isPaused;
    public string NextLevel
    {
        get => nextLevel;
        set => nextLevel = value;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (initialized)
            return;

        initialized = true;
        ResetGame();
        HidePausePanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    public void ResetGame()
    {
        currentLives = maxLives;
        coins = 0;
        SetPauseState(false);
    }

    public void TogglePause()
    {
        SetPauseState(!isPaused);
    }

    public void ResumeGame()
    {
        SetPauseState(false);
    }

    public void ResumeButton()
    {
        ResumeGame();
    }

    public void LoadMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartGame()
    {
        ResumeGame();

        string sceneToLoad = string.IsNullOrWhiteSpace(restartSceneName)
            ? SceneManager.GetActiveScene().name
            : restartSceneName;

        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddLives(int amount)
    {
        currentLives = Mathf.Max(0, currentLives + amount);
    }

    public void AddCoin(int value)
    {
        coins += value;
    }

    public void SetNextLevel(string levelName)
    {
        nextLevel = levelName;
    }

    private void SetPauseState(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        SetPausePanelActive(paused);
    }

    private void SetPausePanelActive(bool active)
    {
        if (pausePanel != null)
            pausePanel.SetActive(active);
    }

    private void HidePausePanel()
    {
        SetPausePanelActive(false);
    }
}
