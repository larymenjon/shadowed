using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public float damageCooldown = 1f;
    [SerializeField] private string gameOverSceneName = "EndGame";

    private int currentLives;
    private bool canTakeDamage = true;
    private PlayerDamageFeedback damageFeedback;

    private void Start()
    {
        damageFeedback = GetComponent<PlayerDamageFeedback>();
        currentLives = maxLives;

        if (UIHealth.Instance != null)
            UIHealth.Instance.UpdateHearts(currentLives);
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage)
            return;

        currentLives -= amount;

        if (UIHealth.Instance != null)
            UIHealth.Instance.UpdateHearts(currentLives);

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            if (damageFeedback != null)
                damageFeedback.Blink();

            StartCoroutine(DamageCooldownRoutine());
        }
    }

    private void Die()
    {
        // Backward compatibility: old prefabs/scenes may still have "GameOver" serialized.
        if (gameOverSceneName == "GameOver")
            gameOverSceneName = "EndGame";

        SceneManager.LoadScene(gameOverSceneName);
    }

    private IEnumerator DamageCooldownRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}
