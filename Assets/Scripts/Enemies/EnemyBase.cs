using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected PlayerController player;
    protected Rigidbody2D playerRb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Object.FindFirstObjectByType<PlayerController>();

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    protected bool IsPlayerMoving()
    {
        if (player == null || playerRb == null)
            return false;

        // Player andando OU pulando
        return player.CurrentMoveInput != 0 ||
               Mathf.Abs(playerRb.linearVelocity.y) > 0.1f;
    }
}



