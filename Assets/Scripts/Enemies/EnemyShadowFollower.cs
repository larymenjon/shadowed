using UnityEngine;

public class EnemyShadowFollower : EnemyBase
{
    public float speed = 2.2f;
    public float minDistance = 0.7f;

    private void FixedUpdate()
    {
        if (player == null || playerRb == null)
            return;

        float playerInput = player.CurrentMoveInput;

        // REGRA ABSOLUTA: player parou = sombra para
        if (Mathf.Abs(playerInput) < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceX = player.transform.position.x - transform.position.x;

        // Evita grudar demais
        if (Mathf.Abs(distanceX) < minDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // SEMPRE em direção ao player
        float direction = Mathf.Sign(distanceX);

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Flip visual
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
}


