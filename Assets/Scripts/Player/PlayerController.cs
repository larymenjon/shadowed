using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 11f;

    [Header("Jump Feel")]
    public float fallMultiplier = 3.5f;
    public float lowJumpMultiplier = 3f;

    // Usado por inimigos
    public float CurrentMoveInput { get; private set; }
    public bool PlayerIsMoving => Mathf.Abs(CurrentMoveInput) > 0.01f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = 1f;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleBetterJump();
    }

    private void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");
        CurrentMoveInput = move;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move > 0) sprite.flipX = false;
        else if (move < 0) sprite.flipX = true;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; // evita double jump acidental
        }
    }

    private void HandleBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    // ✅ DETECÇÃO CORRETA DE CHÃO
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Se a normal aponta pra cima, estamos em cima do chão
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                rb.gravityScale = 1f;
                break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
