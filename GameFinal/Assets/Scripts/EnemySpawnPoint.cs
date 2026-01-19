using UnityEngine;
using System.Collections;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn 设置")]
    [Tooltip("要刷新的敌人 Prefab")]
    public GameObject enemyPrefab;

    [Tooltip("生成次数（-1 表示无限）")]
    public int spawnCount = 1;

    [Tooltip("刷新间隔（秒）")]
    public float spawnInterval = 2f;

    [Tooltip("是否在开始时自动刷新")]
    public bool spawnOnStart = true;

    private int spawned = 0;

    void Start()
    {
        if (spawnOnStart && enemyPrefab != null)
        {
            StartCoroutine(SpawnCoroutine());
        }
    }

    IEnumerator SpawnCoroutine()
    {
        while (spawnCount == -1 || spawned < spawnCount)
        {
            SpawnEnemy();
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(
            enemyPrefab,
            transform.position,
            transform.rotation
        );
    }

    // Scene 中显示刷新点
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
    }
}
