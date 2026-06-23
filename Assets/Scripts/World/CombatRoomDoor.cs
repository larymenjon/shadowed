using System.Collections.Generic;
using UnityEngine;

public class CombatRoomDoor : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool startBlocked = false;

    [Header("Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool useAnimator = true;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private string closeTriggerName = "Close";

    [Header("Physical Block")]
    [SerializeField] private GameObject blockerObject;
    [SerializeField] private List<Collider2D> blockerColliders = new();

    private Collider2D ownCollider;

    public bool IsBlocked { get; private set; }

    private void Awake()
    {
        ownCollider = GetComponent<Collider2D>();
        ApplyState(startBlocked);
    }

    public void SetBlocked(bool blocked)
    {
        ApplyState(blocked);
    }

    public void OpenDoor()
    {
        ApplyState(false);
    }

    public void CloseDoor()
    {
        ApplyState(true);
    }

    private void ApplyState(bool blocked)
    {
        IsBlocked = blocked;

        if (doorAnimator != null && useAnimator)
        {
            if (blocked)
            {
                doorAnimator.ResetTrigger(openTriggerName);
                doorAnimator.SetTrigger(closeTriggerName);
            }
            else
            {
                doorAnimator.ResetTrigger(closeTriggerName);
                doorAnimator.SetTrigger(openTriggerName);
            }
        }

        ApplyPhysicalBlock(blocked);
    }

    private void ApplyPhysicalBlock(bool blocked)
    {
        if (blockerObject != null)
            blockerObject.SetActive(blocked);

        if (blockerColliders != null && blockerColliders.Count > 0)
        {
            for (int i = 0; i < blockerColliders.Count; i++)
            {
                Collider2D blockerCollider = blockerColliders[i];
                if (blockerCollider != null)
                    blockerCollider.enabled = blocked;
            }

            return;
        }

        if (ownCollider != null)
            ownCollider.enabled = blocked;
    }
}
