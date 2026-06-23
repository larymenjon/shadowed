using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 90f;
    [SerializeField] private float airControlPercent = 0.75f;

    [Header("Jump Feel")]
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private int extraJumps = 1;

    [Header("Anti Stick")]
    [SerializeField] private float wallNormalThreshold = 0.6f;
    [SerializeField] private float wallSlideMaxFallSpeed = -14f;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private float attackSpriteDuration = 0.12f;
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Sprite attackSprite;

    [Header("Steps")]
    [SerializeField] private float distanceToCountStep = 2.0f;

    public float CurrentMoveInput { get; private set; }
    public float MaxSpeed => maxSpeed;
    public bool PlayerIsMoving => rb != null && (Mathf.Abs(rb.linearVelocity.x) > 0.05f || Mathf.Abs(rb.linearVelocity.y) > 0.12f);

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool isTouchingWall;
    private bool wantsJump;
    private bool jumpHeld;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private int jumpsRemaining;
    private float moveInput;
    private float distanceCounter;
    private float lastAttackTime = -999f;
    private Sprite defaultSprite;
    private Coroutine attackSpriteRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
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
        CountSteps();
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

        if (moveInput > 0f)
            spriteRenderer.flipX = false;
        else if (moveInput < 0f)
            spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
            wantsJump = true;
        }

        if (Input.GetMouseButtonDown(0))
            TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        PlayAttackSprite();

        Vector2 hitCenter = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, attackRange, attackMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            VampireHealth vampire = hits[i].GetComponentInParent<VampireHealth>();
            if (vampire != null)
                vampire.TakeDamage(attackDamage);
        }
    }

    private void PlayAttackSprite()
    {
        if (attackSprite == null || spriteRenderer == null)
            return;

        if (attackSpriteRoutine != null)
            StopCoroutine(attackSpriteRoutine);

        attackSpriteRoutine = StartCoroutine(AttackSpriteRoutine());
    }

    private IEnumerator AttackSpriteRoutine()
    {
        spriteRenderer.sprite = attackSprite;
        yield return new WaitForSeconds(attackSpriteDuration);
        spriteRenderer.sprite = defaultSprite;
        attackSpriteRoutine = null;
    }

    private void UpdateTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float controlMultiplier = isGrounded ? 1f : airControlPercent;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        accelRate *= controlMultiplier;

        rb.AddForce(Vector2.right * (speedDiff * accelRate), ForceMode2D.Force);
    }

    private void HandleJumpExecution()
    {
        if (!wantsJump || jumpBufferTimer <= 0f)
            return;

        bool canGroundOrCoyoteJump = isGrounded || coyoteTimer > 0f;
        if (canGroundOrCoyoteJump)
        {
            DoJump();
            PlayerStepCounter.Instance?.RegisterJump();
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

    private void CountSteps()
    {
        if (!isGrounded || Mathf.Abs(rb.linearVelocity.x) <= 0.1f)
        {
            distanceCounter = 0f;
            return;
        }

        distanceCounter += Mathf.Abs(rb.linearVelocity.x) * Time.deltaTime;
        if (distanceCounter < distanceToCountStep)
            return;

        PlayerStepCounter.Instance?.RegisterStep();
        distanceCounter = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            EvaluateCollisionContacts(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}
