using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UnstablePlatform : MonoBehaviour
{
    [Header("Step Thresholds")]
    [SerializeField] private int stepsToShake = 10;
    [SerializeField] private int stepsToSlip = 20;
    [SerializeField] private int stepsToCollapse = 30;

    [Header("Shake")]
    [SerializeField] private float shakeAmount = 0.05f;
    [SerializeField] private float shakeSpeed = 20f;

    [Header("Materials")]
    [SerializeField] private PhysicsMaterial2D normalMaterial;
    [SerializeField] private PhysicsMaterial2D slipperyMaterial;

    private Vector3 originalPosition;
    private Collider2D platformCollider;
    private Rigidbody2D platformRigidbody;
    private bool collapsed;

    private void Start()
    {
        originalPosition = transform.position;
        platformCollider = GetComponent<Collider2D>();
        platformRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerStepCounter counter = PlayerStepCounter.Instance;
        if (counter == null || collapsed)
            return;

        int steps = counter.Steps;

        if (steps >= stepsToShake)
            Shake();
        else
            ResetPosition();

        if (platformCollider != null)
            platformCollider.sharedMaterial = steps >= stepsToSlip ? slipperyMaterial : normalMaterial;

        if (steps >= stepsToCollapse)
            Collapse();
    }

    private void Shake()
    {
        float offset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        transform.position = originalPosition + new Vector3(offset, 0f, 0f);
    }

    private void ResetPosition()
    {
        transform.position = originalPosition;
    }

    private void Collapse()
    {
        collapsed = true;

        if (platformCollider != null)
            platformCollider.enabled = false;

        if (platformRigidbody != null)
            platformRigidbody.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
    }
}
