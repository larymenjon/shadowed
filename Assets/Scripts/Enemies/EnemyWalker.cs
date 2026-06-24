using UnityEngine;

public class EnemyWalker : EnemyBase
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDamping = 10f;

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float playerInput = player.CurrentMoveInput;

        if (Mathf.Abs(playerInput) < 0.01f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, stopDamping * Time.fixedDeltaTime);
            return;
        }

        float enemyDirection = -Mathf.Sign(playerInput);
        rb.linearVelocity = new Vector2(enemyDirection * speed, rb.linearVelocity.y);
        FaceDirection(enemyDirection);
    }
}
