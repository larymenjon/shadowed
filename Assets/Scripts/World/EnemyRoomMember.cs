using UnityEngine;

[DisallowMultipleComponent]
public class EnemyRoomMember : MonoBehaviour
{
    private CombatRoomController roomController;
    private bool deathReported;

    public bool IsAlive => !deathReported;

    public void BindToRoom(CombatRoomController controller)
    {
        roomController = controller;
        deathReported = false;
    }

    public void NotifyKilled()
    {
        if (deathReported)
            return;

        deathReported = true;
        roomController?.HandleEnemyKilled(this);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying || deathReported || roomController == null)
            return;

        NotifyKilled();
    }
}
