using UnityEngine;

public class EnemyShadowFollower : EnemyBase
{
    [Header("Chase Balance")]
    [SerializeField] private float minDistance = 1.1f;
    [SerializeField] private float catchupDistance = 6f;
    [SerializeField] private float minSpeed = 3.8f;
    [SerializeField] private float maxSpeed = 5.9f;
    [SerializeField] private float stopDamping = 12f;
    [SerializeField] private float reactionTime = 0.1f;
    [SerializeField] private float acceleration = 18f;

    [Header("Pressure Sprint")]
    [SerializeField] private float sprintDistance = 8f;
    [SerializeField] private float sprintMultiplier = 1.12f;
    [SerializeField] private float sprintDuration = 0.6f;
    [SerializeField] private float sprintCooldown = 2.4f;

    [Header("Fairness")]
    [Range(0.7f, 1f)] [SerializeField] private float normalSpeedVsPlayer = 0.9f;
    [Range(0.8f, 1.2f)] [SerializeField] private float sprintSpeedVsPlayer = 1.02f;

    private float reactionTimer;
    private float sprintTimer;
    private float sprintCooldownTimer;

    private void FixedUpdate()
    {
        if (player == null || playerRb == null)
            return;

        float playerInput = player.CurrentMoveInput;

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

        float verticalClamp = Mathf.Clamp(Mathf.Abs(playerRb.linearVelocity.y), 0f, 3f);
        targetSpeed += verticalClamp * 0.1f;

        sprintCooldownTimer = Mathf.Max(0f, sprintCooldownTimer - Time.fixedDeltaTime);
        sprintTimer = Mathf.Max(0f, sprintTimer - Time.fixedDeltaTime);

        if (Mathf.Abs(distanceX) >= sprintDistance && sprintCooldownTimer <= 0f && sprintTimer <= 0f)
        {
            sprintTimer = sprintDuration;
            sprintCooldownTimer = sprintCooldown;
        }

        if (sprintTimer > 0f)
            targetSpeed *= sprintMultiplier;

        float playerMaxSpeed = Mathf.Max(1f, player.MaxSpeed);
        float maxAllowedByFairness = playerMaxSpeed * (sprintTimer > 0f ? sprintSpeedVsPlayer : normalSpeedVsPlayer);
        targetSpeed = Mathf.Min(targetSpeed, maxAllowedByFairness);

        float targetVelocityX = direction * targetSpeed;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        FaceDirection(direction);
    }
}
