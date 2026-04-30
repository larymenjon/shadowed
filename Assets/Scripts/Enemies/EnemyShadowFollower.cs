using UnityEngine;

public class EnemyShadowFollower : EnemyBase
{
    [Header("Tutorial Balance")]
    public float minDistance = 0.9f;
    public float catchupDistance = 4f;
    public float minSpeed = 1.25f;
    public float maxSpeed = 2.35f;
    public float stopDamping = 12f;
    public float reactionTime = 0.12f;

    private float reactionTimer;

    private void FixedUpdate()
    {
        if (player == null || playerRb == null)
            return;

        float playerInput = player.CurrentMoveInput;

        // Regra: se o player para, a sombra desacelera e para.
        if (Mathf.Abs(playerInput) < 0.01f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, stopDamping * Time.fixedDeltaTime);
            reactionTimer = 0f;
            return;
        }

        reactionTimer += Time.fixedDeltaTime;
        if (reactionTimer < reactionTime)
            return;

        reactionTimer = 0f;

        float distanceX = player.transform.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) < minDistance)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, stopDamping * Time.fixedDeltaTime);
            return;
        }

        float direction = Mathf.Sign(distanceX);
        float distanceFactor = Mathf.Clamp01((Mathf.Abs(distanceX) - minDistance) / Mathf.Max(0.01f, catchupDistance));
        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, distanceFactor);

        // Pequeno bonus quando o player esta em movimento vertical.
        float verticalClamp = Mathf.Clamp(Mathf.Abs(playerRb.linearVelocity.y), 0f, 3f);
        targetSpeed += verticalClamp * 0.1f;

        rb.linearVelocity = new Vector2(direction * targetSpeed, rb.linearVelocity.y);
        FaceDirection(direction);
    }
}
