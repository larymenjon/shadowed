using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnemyAutoSpawner : MonoBehaviour
{
    [Serializable]
    public class LevelSpawnConfig
    {
        public string sceneName;
        public GameObject enemyPrefab;
        public int enemyCount = 1;
    }

    [Header("Config")]
    [SerializeField] private LevelSpawnConfig[] levelConfigs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool destroyExistingEnemiesOnStart = true;

    [Header("Fallback Spawn Area")]
    [SerializeField] private bool useFallbackAreaIfNoSpawnPoints = true;
    [SerializeField] private Vector2 areaSize = new Vector2(8f, 3f);

    private void Start()
    {
        SpawnForCurrentLevel();
    }

    public void SpawnForCurrentLevel()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        LevelSpawnConfig config = FindConfig(activeScene);
        if (config == null || config.enemyPrefab == null)
        {
            Debug.LogWarning("[LevelEnemyAutoSpawner] Sem configuracao de spawn para: " + activeScene);
            return;
        }

        if (destroyExistingEnemiesOnStart)
        {
            EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i].gameObject);
            }
        }

        int amount = Mathf.Max(0, config.enemyCount);
        for (int i = 0; i < amount; i++)
        {
            Vector3 position = ResolveSpawnPosition(i, amount);
            Instantiate(config.enemyPrefab, position, Quaternion.identity);
        }
    }

    private LevelSpawnConfig FindConfig(string sceneName)
    {
        if (levelConfigs == null)
            return null;

        for (int i = 0; i < levelConfigs.Length; i++)
        {
            if (levelConfigs[i] != null && levelConfigs[i].sceneName == sceneName)
                return levelConfigs[i];
        }

        return null;
    }

    private Vector3 ResolveSpawnPosition(int index, int total)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[index % spawnPoints.Length];
            if (point != null)
                return point.position;
        }

        if (!useFallbackAreaIfNoSpawnPoints)
            return transform.position;

        float normalized = total <= 1 ? 0.5f : (float)index / (total - 1);
        float x = Mathf.Lerp(-areaSize.x * 0.5f, areaSize.x * 0.5f, normalized);
        float y = UnityEngine.Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
        return transform.position + new Vector3(x, y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
