using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthBar;
    [SerializeField] private bool dropKeyOnDeath = true;
    [SerializeField] private bool onlyDropOnLevel1 = true;
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform keySpawnPoint;

    private int currentHealth;
    private bool dead;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        currentHealth -= Mathf.Max(0, damage);
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        dead = true;
        TryDropKey();
        Destroy(gameObject);
    }

    private void TryDropKey()
    {
        if (!dropKeyOnDeath || keyPrefab == null)
            return;

        if (onlyDropOnLevel1 && SceneManager.GetActiveScene().name != level1SceneName)
            return;

        Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position;
        Instantiate(keyPrefab, spawnPos, Quaternion.identity);
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.minValue = 0f;
        healthBar.maxValue = maxHealth;
        healthBar.value = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
