using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerDamageFeedback : MonoBehaviour
{
    public float blinkInterval = 0.1f;
    public int blinkCount = 6;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Blink()
    {
        StartCoroutine(BlinkRoutine());
    }

    System.Collections.IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        sr.enabled = true;
    }
}

