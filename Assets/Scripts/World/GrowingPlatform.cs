using UnityEngine;

public class GrowingPlatform : MonoBehaviour
{
    [Header("Growth")]
    [SerializeField] private float growPerStep = 0.08f;
    [SerializeField] private float maxWidth = 12f;

    [Header("Shrink By Jumps")]
    [SerializeField] private int jumpsToShrink = 3;
    [SerializeField] private float shrinkAmount = 0.8f;

    private Vector3 initialScale;
    private bool playerOnPlatform;
    private int lastStepCount;
    private int jumpCheckpoint;

    private void Start()
    {
        initialScale = transform.localScale;
        CacheCounterSnapshot();
    }

    private void Update()
    {
        if (PlayerStepCounter.Instance == null)
            return;

        HandleGrowth();

        if (playerOnPlatform)
            HandleShrinkByJump();
    }

    private void CacheCounterSnapshot()
    {
        PlayerStepCounter counter = PlayerStepCounter.Instance;
        lastStepCount = counter != null ? counter.Steps : 0;
        jumpCheckpoint = counter != null ? counter.Jumps : 0;
    }

    private void HandleGrowth()
    {
        PlayerStepCounter counter = PlayerStepCounter.Instance;
        if (counter == null)
            return;

        int currentSteps = counter.Steps;
        if (currentSteps <= lastStepCount)
            return;

        int diff = currentSteps - lastStepCount;
        Vector3 scale = transform.localScale;
        scale.x += diff * growPerStep * scale.x;
        scale.x = Mathf.Clamp(scale.x, initialScale.x, maxWidth);
        transform.localScale = scale;
        lastStepCount = currentSteps;
    }

    private void HandleShrinkByJump()
    {
        PlayerStepCounter counter = PlayerStepCounter.Instance;
        if (counter == null)
            return;

        int jumpsNow = counter.Jumps - jumpCheckpoint;
        if (jumpsNow < jumpsToShrink)
            return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Max(scale.x - shrinkAmount, initialScale.x);
        transform.localScale = scale;
        jumpCheckpoint = counter.Jumps;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerOnPlatform = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerOnPlatform = false;
    }
}
