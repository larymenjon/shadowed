using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CombatRoomTrigger : MonoBehaviour
{
    [SerializeField] private CombatRoomController roomController;
    [SerializeField] private bool autoFindRoomControllerInParents = true;

    private void Awake()
    {
        ResolveRoomController();
    }

    private void Reset()
    {
        ResolveRoomController();
    }

    private void OnValidate()
    {
        ResolveRoomController();
    }

    private void ResolveRoomController()
    {
        if (roomController != null || !autoFindRoomControllerInParents)
            return;

        roomController = GetComponentInParent<CombatRoomController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (roomController == null || !other.CompareTag("Player"))
            return;

        roomController.StartRoom();
    }
}
