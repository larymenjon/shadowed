using UnityEngine;

public class GrowingPlatform : MonoBehaviour
{
    [Header("Crescimento")]
    public float growPerStep = 0.08f;
    public float maxWidth = 12f;

    [Header("Redução por pulos")]
    public int jumpsToShrink = 3;
    public float shrinkAmount = 0.8f;

    private Vector3 initialScale;
    private bool playerOnPlatform;

    private int lastStepCount;
    private int jumpCheckpoint;

    void Start()
    {
        initialScale = transform.localScale;

        // Contagem GLOBAL desde o início da fase
        lastStepCount = PlayerStepCounter.instance.steps;
        jumpCheckpoint = PlayerStepCounter.instance.jumps;
    }

    void Update()
    {
        HandleGrowth();

        if (playerOnPlatform)
            HandleShrinkByJump();
    }

    void HandleGrowth()
    {
        int currentSteps = PlayerStepCounter.instance.steps;

        if (currentSteps > lastStepCount)
        {
            int diff = currentSteps - lastStepCount;

            Vector3 scale = transform.localScale;
            //scale.x += diff * growPerStep;
            scale.x += diff * growPerStep * scale.x;
            scale.x = Mathf.Clamp(scale.x, initialScale.x, maxWidth);

            transform.localScale = scale;
            lastStepCount = currentSteps;
        }
    }

    void HandleShrinkByJump()
    {
        int jumpsNow = PlayerStepCounter.instance.jumps - jumpCheckpoint;

        if (jumpsNow >= jumpsToShrink)
        {
            Vector3 scale = transform.localScale;
            scale.x -= shrinkAmount;
            scale.x = Mathf.Max(scale.x, initialScale.x);

            transform.localScale = scale;

            jumpCheckpoint = PlayerStepCounter.instance.jumps;

            Debug.Log("Plataforma DIMINUIU por pulos");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerOnPlatform = true;

        Debug.Log("Player entrou na plataforma");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerOnPlatform = false;

        Debug.Log("Player saiu da plataforma");
    }
}
