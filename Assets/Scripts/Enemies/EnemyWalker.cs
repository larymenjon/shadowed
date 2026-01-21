using UnityEngine;

public class EnemyWalker : EnemyBase
{
    public float speed = 2f;

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float playerInput = player.CurrentMoveInput;

        if (Mathf.Abs(playerInput) < 0.01f)
        {
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                Vector2.zero,
                10f * Time.fixedDeltaTime
            );
            return;
        }

        // MOVIMENTO ESPELHADO
        float enemyDirection = -Mathf.Sign(playerInput);

        rb.linearVelocity = new Vector2(enemyDirection * speed, rb.linearVelocity.y);

        // Flip visual
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * enemyDirection;
        transform.localScale = scale;
    }
}




