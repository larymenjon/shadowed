using UnityEngine;

public class PlayerStepCounter : MonoBehaviour
{
    public static PlayerStepCounter instance;

    [Header("Debug")]
    public int steps;
    public int jumps;

    [Header("Config")]
    public float stepInterval = 0.25f;

    private float stepTimer;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckMovement();
        CheckJump();
    }

    void CheckMovement()
    {
        // Setas do teclado
        bool moving =
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow);

        if (moving && isGrounded)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                steps++;
                stepTimer = 0f;

                Debug.Log("PASSO contado > Total: " + steps);
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumps++;
            Debug.Log("PULO contado > Total: " + jumps);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("TOCOU NO CHAO");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("SAIU DO CHAO");
        }
    }
}
