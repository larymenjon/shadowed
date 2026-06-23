using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [Header("Pause Panel Slots")]
    [SerializeField] private Transform optionsPanelSlot;
    [SerializeField] private Transform controlsPanelSlot;

    private bool paused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        SetPauseState(!paused);
    }

    public void Resume()
    {
        SetPauseState(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public Transform GetOptionsPanelSlot()
    {
        return optionsPanelSlot;
    }

    public Transform GetControlsPanelSlot()
    {
        return controlsPanelSlot;
    }

    private void SetPauseState(bool isPaused)
    {
        paused = isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }
}
