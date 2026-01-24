using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform player, destino;
    public GameObject playerObj;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerObj.SetActive(false);
            player.position = destino.position;
            playerObj.SetActive(true);
        }
    }
}
