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
    [SerializeField] private EnemyData[] enemyTypes;
    [Tooltip("Drag your ‘Enemies’ container here")]
    public Transform enemyContainer;

    private List<Enemy> active = new List<Enemy>();
    int enemyLayer;

    [SerializeField] private EnemyData[] allEnemies;
    private List<EnemyData> availableEnemies = new();

    [System.Serializable]
    public class EnemyData
    {
        public GameObject prefab;
        public int rewardCoins = 1;
        public int damage = 1;

    }

    private int enemiesRemainingInWave;

    private void Awake()
    {
        I = this;
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

public IEnumerator SpawnWave(int enemyCount, float enemySpeed, float spawnInterval)
{
    enemiesRemainingInWave = enemyCount;
    GameManager.I.UpdateEnemiesLeftCounter();

    // --- Handle unlocking new enemies ---
    UpdateAvailableEnemies();

    for (int i = 0; i < enemyCount; i++)
    {
        var chosenType = availableEnemies[UnityEngine.Random.Range(0, availableEnemies.Count)];

        var go = Instantiate(chosenType.prefab, spawnPoint.position, Quaternion.identity);

        if (enemyContainer != null)
            go.transform.SetParent(enemyContainer, worldPositionStays: true);

        SetLayerRecursively(go, enemyLayer);

        var e = go.GetComponent<Enemy>();
        float baseSpeed = e.speed;
        float multiplier = GetEnemyStatMultiplier();
        float scaledSpeed = baseSpeed * multiplier;

        e.Init(scaledSpeed, Mathf.RoundToInt(chosenType.rewardCoins * multiplier), Mathf.RoundToInt(chosenType.damage * multiplier));

        active.Add(e);
        GameManager.I.UpdateEnemiesLeftCounter();
        yield return new WaitForSeconds(spawnInterval);
    }

    while (active.Count > 0)
        yield return null;

    GameManager.I.WaveComplete();
}

private void UpdateAvailableEnemies()
{
    int wave = GameManager.I.currentWave;

    if (wave == 1 && availableEnemies.Count == 0)
    {
        availableEnemies.Add(allEnemies[0]); // Novice Zombie
    }

    if (wave == 3 && !availableEnemies.Contains(allEnemies[3]))
    {
        availableEnemies.Add(allEnemies[3]); // Novice Skeleton
    }

    if (wave == 5 && !availableEnemies.Contains(allEnemies[6]))
    {
        availableEnemies.Add(allEnemies[6]); // Novice Minotaur
    }

    if (wave == 8 && !availableEnemies.Contains(allEnemies[1]))
    {
        availableEnemies.Add(allEnemies[1]); // Adept Zombie
    }

    if (wave == 11 && !availableEnemies.Contains(allEnemies[4]))
    {
        availableEnemies.Add(allEnemies[4]); // Adept Skeleton
    }

    if (wave == 14 && !availableEnemies.Contains(allEnemies[7]))
    {
        availableEnemies.Add(allEnemies[7]); // Adept Minotaur
    }

    if (wave == 17 && !availableEnemies.Contains(allEnemies[2]))
    {
        availableEnemies.Add(allEnemies[2]); // Veteran Zombie
    }

    if (wave == 20 && !availableEnemies.Contains(allEnemies[5]))
    {
        availableEnemies.Add(allEnemies[5]); // Veteran Skeleton
    }

    if (wave == 23 && !availableEnemies.Contains(allEnemies[8]))
    {
        availableEnemies.Add(allEnemies[8]); // Veteran Minotaur
    }
}

private float GetEnemyStatMultiplier()
{
    int wave = GameManager.I.currentWave;
    return Mathf.Pow(1.5f, wave / 2);
}

public void NotifyEnemyDeath(Enemy e)
{
    active.Remove(e);
        enemiesRemainingInWave--;
    GameManager.I.UpdateEnemiesLeftCounter();
}

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public int GetAliveEnemyCount()
{
    return Mathf.Max(enemiesRemainingInWave, 0);
}
}
