using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6.25f;
    public float jumpForce = 10.25f;

    [Header("Jump Feel")]
    public float fallMultiplier = 3.1f;
    public float lowJumpMultiplier = 2.7f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    // Usado por inimigos
    public float CurrentMoveInput { get; private set; }
    public bool PlayerIsMoving => rb != null && (Mathf.Abs(rb.linearVelocity.x) > 0.05f || Mathf.Abs(rb.linearVelocity.y) > 0.12f);

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isGrounded;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = 1f;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpInput();
        HandleJumpExecution();
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

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        jumpBufferTimer -= Time.deltaTime;
        coyoteTimer -= Time.deltaTime;
    }

    private void HandleJumpExecution()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
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
                coyoteTimer = coyoteTime;
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
                coyoteTimer = coyoteTime;
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
