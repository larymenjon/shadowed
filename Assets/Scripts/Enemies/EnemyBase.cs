using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected PlayerController player;
    protected Rigidbody2D playerRb;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = Object.FindFirstObjectByType<PlayerController>();

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    protected bool IsPlayerMoving()
    {
        if (player == null || playerRb == null)
            return false;

        return player.CurrentMoveInput != 0f || Mathf.Abs(playerRb.linearVelocity.y) > 0.1f;
    }

    protected void FaceDirection(float direction)
    {
        if (Mathf.Abs(direction) < 0.01f)
            return;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0f;
            return;
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }
}
