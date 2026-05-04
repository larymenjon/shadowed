using UnityEngine;

public class EnemyShadowFollower : EnemyBase
{
    [Header("Chase Balance")]
    public float minDistance = 1.1f;
    public float catchupDistance = 6f;
    public float minSpeed = 3.8f;
    public float maxSpeed = 5.9f;
    public float stopDamping = 12f;
    public float reactionTime = 0.1f;
    public float acceleration = 18f;

    [Header("Pressure Sprint")]
    public float sprintDistance = 8f;
    public float sprintMultiplier = 1.12f;
    public float sprintDuration = 0.6f;
    public float sprintCooldown = 2.4f;

    [Header("Fairness")]
    [Range(0.7f, 1f)] public float normalSpeedVsPlayer = 0.9f;
    [Range(0.8f, 1.2f)] public float sprintSpeedVsPlayer = 1.02f;

    private float reactionTimer;
    private float sprintTimer;
    private float sprintCooldownTimer;

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
            sprintTimer = 0f;
            sprintCooldownTimer = 0f;
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

        // Sprint curto para dar pressao quando o player abre muita distancia.
        sprintCooldownTimer = Mathf.Max(0f, sprintCooldownTimer - Time.fixedDeltaTime);
        sprintTimer = Mathf.Max(0f, sprintTimer - Time.fixedDeltaTime);

        if (Mathf.Abs(distanceX) >= sprintDistance && sprintCooldownTimer <= 0f && sprintTimer <= 0f)
        {
            sprintTimer = sprintDuration;
            sprintCooldownTimer = sprintCooldown;
        }

        if (sprintTimer > 0f)
            targetSpeed *= sprintMultiplier;

        // Limite de velocidade relativo ao player para manter fuga possivel.
        float playerMaxSpeed = Mathf.Max(1f, player.maxSpeed);
        float maxAllowedByFairness = playerMaxSpeed * (sprintTimer > 0f ? sprintSpeedVsPlayer : normalSpeedVsPlayer);
        targetSpeed = Mathf.Min(targetSpeed, maxAllowedByFairness);

        float targetVelocityX = direction * targetSpeed;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        FaceDirection(direction);
    }
}
