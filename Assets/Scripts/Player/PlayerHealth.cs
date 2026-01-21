using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Adicionado para facilitar o uso de Coroutines

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public float damageCooldown = 1f;

    private int currentLives;
    private bool canTakeDamage = true;

    private void Start()
    {
        currentLives = maxLives;
        if (UIHealth.Instance != null) // Prevenção de erro caso a UI não esteja na cena
            UIHealth.Instance.UpdateHearts(currentLives);
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage) return;

        currentLives -= amount;

        if (UIHealth.Instance != null)
            UIHealth.Instance.UpdateHearts(currentLives);

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageCooldownRoutine());
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(2); // GAME OVER
    }

    // Renomeado para evitar confusão com a variável damageCooldown
    private IEnumerator DamageCooldownRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}


