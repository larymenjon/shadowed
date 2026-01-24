using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : MonoBehaviour
{
    [Header("Flow")]
    public string transitionScene; // PassGame ou EndGame
    public string nextLevel;        // vazio no fim do jogo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            // Se existir próximo nível, salva
            if (!string.IsNullOrEmpty(nextLevel))
            {
                GameManager.Instance.nextLevel = nextLevel;
            }
            SceneManager.LoadScene(transitionScene);

        }
    }
}
