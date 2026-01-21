using UnityEngine;

public class EnemyJumper : EnemyBase
{
    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float jumpCooldown = 1.5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float timer;

    private void Update()
    {
        // Alterado para usar a variável 'player' (da classe base) 
        // e a propriedade 'PlayerIsMoving' que adicionamos no PlayerController
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
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            0.2f,
            groundLayer
        );
    }
}
