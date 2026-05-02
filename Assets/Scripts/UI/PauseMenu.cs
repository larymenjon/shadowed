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
        paused = !paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;
    }

    public void Resume()
    {
        paused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
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
}
