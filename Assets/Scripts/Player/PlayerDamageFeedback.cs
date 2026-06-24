using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerDamageFeedback : MonoBehaviour
{
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private int blinkCount = 6;

    private SpriteRenderer spriteRenderer;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Blink()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        blinkRoutine = null;
    }
}
