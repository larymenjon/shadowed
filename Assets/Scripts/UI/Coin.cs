using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    public int value = 1;
    public AudioClip collectSound;

    private bool collected;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(value);
        }

        if (collectSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                Camera.main.transform.position
            );
        }

        Destroy(gameObject);
    }
}

