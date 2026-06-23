using UnityEngine;

public class PlayerStepCounter : MonoBehaviour
{
    public static PlayerStepCounter Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private int steps;
    [SerializeField] private int jumps;

    [Header("Config")]
    [SerializeField] private float stepInterval = 0.25f;

    private float stepTimer;
    private bool isGrounded;

    public int Steps => steps;
    public int Jumps => jumps;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        CheckMovement();
        CheckJump();
    }

    public void RegisterStep()
    {
        steps++;
    }

    public void RegisterJump()
    {
        jumps++;
    }

    private void CheckMovement()
    {
        if (!IsMovingHorizontally() || !isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer += Time.deltaTime;

        if (stepTimer < stepInterval)
            return;

        RegisterStep();
        stepTimer = 0f;
        Debug.Log("PASSO contado > Total: " + steps);
    }

    private bool IsMovingHorizontally()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
    }

    private void CheckJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !isGrounded)
            return;

        RegisterJump();
        Debug.Log("PULO contado > Total: " + jumps);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
