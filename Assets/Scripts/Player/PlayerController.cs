using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [FormerlySerializedAs("speed")]
    public float maxSpeed = 7f;
    public float acceleration = 70f;
    public float deceleration = 90f;
    public float airControlPercent = 0.75f;

    [Header("Jump Feel")]
    public float jumpForce = 11f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    public int extraJumps = 1;

    [Header("Anti Stick")]
    public float wallNormalThreshold = 0.6f;
    public float wallSlideMaxFallSpeed = -14f;

    // Usado por inimigos
    public float CurrentMoveInput { get; private set; }
    public bool PlayerIsMoving => rb != null && (Mathf.Abs(rb.linearVelocity.x) > 0.05f || Mathf.Abs(rb.linearVelocity.y) > 0.12f);

    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool isGrounded;
    private bool isTouchingWall;
    private bool wantsJump;
    private bool jumpHeld;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private int jumpsRemaining;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = 1f;
    }

    private void Start()
    {
        jumpsRemaining = extraJumps;
    }

    private void Update()
    {
        ReadInput();
        UpdateTimers();
        HandleJumpExecution();
        HandleBetterJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyWallAntiStick();
    }

    private void ReadInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        CurrentMoveInput = moveInput;
        jumpHeld = Input.GetKey(KeyCode.Space);

        if (moveInput > 0f) sprite.flipX = false;
        else if (moveInput < 0f) sprite.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
            wantsJump = true;
        }
    }

    private void UpdateTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float controlMultiplier = isGrounded ? 1f : airControlPercent;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        accelRate *= controlMultiplier;

        float movement = speedDiff * accelRate;
        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);
    }

    private void HandleJumpExecution()
    {
        if (!wantsJump || jumpBufferTimer <= 0f)
            return;

        bool canGroundOrCoyoteJump = isGrounded || coyoteTimer > 0f;

        if (canGroundOrCoyoteJump)
        {
            DoJump();
            coyoteTimer = 0f;
            return;
        }

        if (jumpsRemaining > 0)
        {
            DoJump();
            jumpsRemaining--;
        }
    }

    private void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        isGrounded = false;
        wantsJump = false;
        jumpBufferTimer = 0f;
    }

    private void HandleBetterJump()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void ApplyWallAntiStick()
    {
        if (!isTouchingWall || isGrounded)
            return;

        if (rb.linearVelocity.y < wallSlideMaxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideMaxFallSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        EvaluateCollisionContacts(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        EvaluateCollisionContacts(collision);
    }

    private void EvaluateCollisionContacts(Collision2D collision)
    {
        bool foundGround = false;
        bool foundWall = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                foundGround = true;

            if (Mathf.Abs(contact.normal.x) > wallNormalThreshold && contact.normal.y < 0.4f)
                foundWall = true;
        }

        if (foundGround)
        {
            isGrounded = true;
            coyoteTimer = coyoteTime;
            jumpsRemaining = extraJumps;
            rb.gravityScale = 1f;
        }

        isTouchingWall = foundWall;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        isGrounded = false;
        isTouchingWall = false;
    }
}
