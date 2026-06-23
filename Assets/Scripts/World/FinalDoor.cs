using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : MonoBehaviour
{
    [Header("Flow")]
    [SerializeField] private string transitionScene; // PassGame ou EndGame
    [SerializeField] private string nextLevel;        // vazio no fim do jogo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextLevel) && GameManager.Instance != null)
            GameManager.Instance.NextLevel = nextLevel;

        SceneManager.LoadScene(transitionScene);
    }
}
