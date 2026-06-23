using UnityEngine;

public class StepPlatform : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField] private int stepsToAppear = 6;
    [SerializeField] private int jumpsToDisappear = 5;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isVisible = true;
    private int stepCheckpoint;
    private int jumpCheckpoint;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        PlayerStepCounter counter = PlayerStepCounter.Instance;
        if (counter != null)
            jumpCheckpoint = counter.Jumps;

        ShowPlatform();
    }

    private void Update()
    {
        PlayerStepCounter counter = PlayerStepCounter.Instance;
        if (counter == null)
            return;

        int stepsSinceHide = Mathf.Max(0, counter.Steps - stepCheckpoint);
        int jumpsSinceShow = Mathf.Max(0, counter.Jumps - jumpCheckpoint);

        if (!isVisible && stepsSinceHide >= stepsToAppear)
        {
            isVisible = true;
            jumpCheckpoint = counter.Jumps;
            ShowPlatform();
        }

        if (isVisible && jumpsSinceShow >= jumpsToDisappear)
        {
            isVisible = false;
            stepCheckpoint = counter.Steps;
            HidePlatform();
        }
    }

    private void ShowPlatform()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (platformCollider != null)
            platformCollider.enabled = true;
    }

    private void HidePlatform()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (platformCollider != null)
            platformCollider.enabled = false;
    }
}
