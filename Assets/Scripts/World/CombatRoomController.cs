using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatRoomController : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool activateOnlyOnce = true;
    [SerializeField] private bool spawnEnemies = false;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> existingEnemies = new();
    [SerializeField] private List<GameObject> enemyPrefabs = new();
    [SerializeField] private List<Transform> spawnPoints = new();

    [Header("Doors")]
    [SerializeField] private List<CombatRoomDoor> doors = new();

    [Header("Events")]
    [SerializeField] private UnityEvent OnRoomStarted = new();
    [SerializeField] private UnityEvent OnRoomCleared = new();

    [Header("Optional")]
    [SerializeField] private bool lockCamera = false;
    [SerializeField] private Transform cameraLockTarget;
    [SerializeField] private bool deactivateExistingEnemiesOnAwake = true;
    [SerializeField] private float resetDelay = 0.25f;

    private readonly HashSet<EnemyRoomMember> activeMembers = new();
    private readonly List<GameObject> spawnedEnemies = new();

    private CameraFollow cameraFollow;
    private Transform cachedCameraTarget;
    private bool cameraIsLocked;
    private bool roomStarted;
    private bool roomCleared;
    private bool resettingRoom;

    public bool HasStarted => roomStarted;
    public bool IsCleared => roomCleared;

    private void Awake()
    {
        CacheCameraFollow();
        PrepareExistingEnemies();
    }

    private void CacheCameraFollow()
    {
        cameraFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;

        if (cameraFollow == null)
            cameraFollow = Object.FindFirstObjectByType<CameraFollow>();
    }

    private void PrepareExistingEnemies()
    {
        if (!deactivateExistingEnemiesOnAwake || existingEnemies == null)
            return;

        for (int i = 0; i < existingEnemies.Count; i++)
        {
            GameObject enemy = existingEnemies[i];
            if (enemy == null)
                continue;

            if (enemy.activeSelf)
                enemy.SetActive(false);
        }
    }

    public void StartRoom()
    {
        if (resettingRoom)
            return;

        if (roomStarted && !roomCleared)
            return;

        if (roomCleared && activateOnlyOnce)
            return;

        if (roomCleared && !activateOnlyOnce)
            ResetRuntimeState();

        roomStarted = true;
        roomCleared = false;

        CloseDoors();
        LockCamera();
        ActivateExistingEnemies();
        SpawnRoomEnemies();

        OnRoomStarted?.Invoke();

        EvaluateClearState();
    }

    public void ResetRoom()
    {
        StopAllCoroutines();
        ResetRuntimeState();
        UnlockCamera();
        OpenDoors();
    }

    public void HandleEnemyKilled(EnemyRoomMember member)
    {
        if (member == null)
            return;

        activeMembers.Remove(member);
        EvaluateClearState();
    }

    private void ActivateExistingEnemies()
    {
        if (existingEnemies == null)
            return;

        for (int i = 0; i < existingEnemies.Count; i++)
        {
            GameObject enemy = existingEnemies[i];
            if (enemy == null)
                continue;

            RegisterEnemy(enemy);

            if (!enemy.activeSelf)
                enemy.SetActive(true);
        }
    }

    private void SpawnRoomEnemies()
    {
        if (!spawnEnemies || enemyPrefabs == null || enemyPrefabs.Count == 0)
            return;

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            GameObject prefab = enemyPrefabs[i];
            if (prefab == null)
                continue;

            Transform spawnPoint = ResolveSpawnPoint(i);
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            GameObject spawnedEnemy = Instantiate(prefab, spawnPosition, spawnRotation);
            spawnedEnemies.Add(spawnedEnemy);
            RegisterEnemy(spawnedEnemy);
        }
    }

    private Transform ResolveSpawnPoint(int index)
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return null;

        return spawnPoints[index % spawnPoints.Count];
    }

    private void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        EnemyRoomMember member = enemy.GetComponentInChildren<EnemyRoomMember>(true);
        if (member == null)
            member = enemy.AddComponent<EnemyRoomMember>();

        member.BindToRoom(this);
        activeMembers.Add(member);
    }

    private void EvaluateClearState()
    {
        if (!roomStarted || roomCleared)
            return;

        if (activeMembers.Count > 0)
            return;

        ClearRoom();
    }

    private void ClearRoom()
    {
        if (roomCleared)
            return;

        roomCleared = true;
        roomStarted = false;

        OpenDoors();
        UnlockCamera();
        OnRoomCleared?.Invoke();

        if (resetDelay > 0f)
            StartCoroutine(ResetRoomRoutine());
    }

    private IEnumerator ResetRoomRoutine()
    {
        resettingRoom = true;
        yield return new WaitForSeconds(resetDelay);
        resettingRoom = false;

        if (!activateOnlyOnce)
            ResetRuntimeState();
    }

    private void ResetRuntimeState()
    {
        activeMembers.Clear();
        roomStarted = false;
        roomCleared = false;

        if (spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                GameObject spawnedEnemy = spawnedEnemies[i];
                if (spawnedEnemy != null)
                    Destroy(spawnedEnemy);
            }

            spawnedEnemies.Clear();
        }

        if (existingEnemies != null)
        {
            for (int i = 0; i < existingEnemies.Count; i++)
            {
                GameObject enemy = existingEnemies[i];
                if (enemy == null)
                    continue;

                if (!enemy.activeSelf)
                    enemy.SetActive(true);
            }
        }
    }

    private void CloseDoors()
    {
        SetDoorsBlocked(true);
    }

    private void OpenDoors()
    {
        SetDoorsBlocked(false);
    }

    private void SetDoorsBlocked(bool blocked)
    {
        if (doors == null)
            return;

        for (int i = 0; i < doors.Count; i++)
        {
            CombatRoomDoor door = doors[i];
            if (door == null)
                continue;

            door.SetBlocked(blocked);
        }
    }

    private void LockCamera()
    {
        if (!lockCamera || cameraFollow == null || cameraIsLocked)
            return;

        cachedCameraTarget = cameraFollow.Target;
        cameraFollow.Target = cameraLockTarget != null ? cameraLockTarget : transform;
        cameraIsLocked = true;
    }

    private void UnlockCamera()
    {
        if (!cameraIsLocked || cameraFollow == null)
            return;

        cameraFollow.Target = cachedCameraTarget;
        cameraIsLocked = false;
    }
}
