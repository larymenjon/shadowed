using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private string gameOverSceneName = "EndGame";

    private int currentLives;
    private bool canTakeDamage = true;
    private PlayerDamageFeedback damageFeedback;

    private void Start()
    {
        damageFeedback = GetComponent<PlayerDamageFeedback>();
        currentLives = maxLives;
        UIHealth.Instance?.UpdateHearts(currentLives);
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage)
            return;

        currentLives = Mathf.Max(0, currentLives - amount);
        UIHealth.Instance?.UpdateHearts(currentLives);

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        damageFeedback?.Blink();
        StartCoroutine(DamageCooldownRoutine());
    }

    private void Die()
    {
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
