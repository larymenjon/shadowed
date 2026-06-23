using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private AudioClip collectSound;

    private bool collected;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoin(value);

        if (collectSound != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);

        Destroy(gameObject);
    }
}
