using UnityEngine;

public class UnstablePlatform : MonoBehaviour
{
    [Header("Passos da Fase")]
    public int stepsToShake = 10;
    public int stepsToSlip = 20;
    public int stepsToCollapse = 30;

    [Header("Tremor")]
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 20f;

    [Header("Materiais")]
    public PhysicsMaterial2D normalMaterial;
    public PhysicsMaterial2D slipperyMaterial;

    private Vector3 originalPosition;
    private Collider2D col;
    private bool collapsed = false;

    void Start()
    {
        originalPosition = transform.position;
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        int steps = PlayerStepCounter.instance.steps;

        if (collapsed)
            return;

        // TREMER
        if (steps >= stepsToShake)
        {
            Shake();
        }
        else
        {
            ResetPosition();
        }

        // ESCORREGAR
        if (steps >= stepsToSlip)
        {
            col.sharedMaterial = slipperyMaterial;
        }
        else
        {
            col.sharedMaterial = normalMaterial;
        }

        // COLAPSAR
        if (steps >= stepsToCollapse)
        {
            Collapse();
        }
    }

    void Shake()
    {
        float offset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        transform.position = originalPosition + new Vector3(offset, 0f, 0f);
    }

    void ResetPosition()
    {
        transform.position = originalPosition;
    }

    void Collapse()
    {
        collapsed = true;
        col.enabled = false;

        Debug.Log("Plataforma colapsou!");

        // efeito simples de queda
        GetComponent<Rigidbody2D>()?.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
    }
}
