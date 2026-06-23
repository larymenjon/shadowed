using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform destination;
    [SerializeField] private GameObject playerObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || destination == null)
            return;

        Transform target = playerTarget != null ? playerTarget : other.transform;
        GameObject targetObject = playerObject != null ? playerObject : other.gameObject;

        targetObject.SetActive(false);
        target.position = destination.position;
        targetObject.SetActive(true);
    }
}
