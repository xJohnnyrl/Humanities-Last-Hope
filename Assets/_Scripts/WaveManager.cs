using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I { get; private set; }
    public Transform[] checkpoints;  
    public Transform spawnPoint;

    public GameObject enemyPrefab;

    private List<Enemy> active = new List<Enemy>();

    private void Awake() => I = this;

    public IEnumerator SpawnWave(int enemyCount, float enemySpeed, float spawnInterval)
    {
        // spawn loop
        for (int i = 0; i < enemyCount; i++)
        {
            var sp = spawnPoint;
            var go = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            var e = go.GetComponent<Enemy>();
            e.Init(speed: enemySpeed);
            active.Add(e);
            yield return new WaitForSeconds(spawnInterval);
        }

        // wait until all enemies are gone
        while (active.Count > 0)
            yield return null;

        GameManager.I.WaveComplete();
    }

    public void NotifyEnemyDeath(Enemy e) => active.Remove(e);
}
