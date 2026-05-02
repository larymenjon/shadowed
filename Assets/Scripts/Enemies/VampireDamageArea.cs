using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VampireDamageArea : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 0.8f;

    private float nextDamageTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + damageInterval;
    }
}
