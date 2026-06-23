using UnityEngine;

public class EnemyJumper : EnemyBase
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpCooldown = 1.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float timer;

    private void Update()
    {
        if (player == null || !player.PlayerIsMoving)
            return;

        timer += Time.deltaTime;

        if (timer >= jumpCooldown && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            timer = 0f;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
