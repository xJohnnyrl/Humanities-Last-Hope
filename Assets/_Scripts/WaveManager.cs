using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I { get; private set; }

    [Header("Pathing")]
    public Transform[] checkpoints;  
    public Transform spawnPoint;

    [Header("Spawning")]
    public GameObject enemyPrefab;
    [Tooltip("Drag your ‘Enemies’ container here")]
    public Transform enemyContainer;

    private List<Enemy> active = new List<Enemy>();
    int enemyLayer;

    private void Awake()
    {
        I = this;
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    public IEnumerator SpawnWave(int enemyCount, float enemySpeed, float spawnInterval)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // 1) instantiate
            var go = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // 2) parent under your Enemies container
            if (enemyContainer != null)
                go.transform.SetParent(enemyContainer, worldPositionStays: true);

            // 3) set the correct layer on root + all children
            SetLayerRecursively(go, enemyLayer);

            // 4) init speed
            var e = go.GetComponent<Enemy>();
            float baseSpeed = e.speed;  
            float scaled    = baseSpeed * (1 + GameManager.I.currentWave * 0.1f);
            e.Init(scaled);

            active.Add(e);
            yield return new WaitForSeconds(spawnInterval);
        }

        // wait for them all to die
        while (active.Count > 0)
            yield return null;

        GameManager.I.WaveComplete();
    }

    public void NotifyEnemyDeath(Enemy e) => active.Remove(e);

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
