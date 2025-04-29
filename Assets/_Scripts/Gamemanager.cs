using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameoverscreen;
    [SerializeField] GameObject darkbackground;
    public static GameManager I { get; private set; }
    public int coins = 100;
    public int health = 10;
    public int currentWave = 0;
    public float spawnInterval = 10f;
    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action OnEnterShop;
    public event Action OnStatsChanged;
    [SerializeField] private TMP_Text waveCounterText;
    [SerializeField] private TMP_Text enemiesLeftText;
    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        NextWave();
    }

    public void NextWave()
    {
        currentWave++;
        UpdateWaveCounter();
        UpdateEnemiesLeftCounter();

        OnWaveStarted?.Invoke();
        StartCoroutine(WaveManager.I.SpawnWave(
            enemyCount: 5 + currentWave * 2,
            enemySpeed: 2f + currentWave * 0.2f,
            spawnInterval: spawnInterval
        ));
    }
    public void WaveComplete()
    {
        UpdateEnemiesLeftCounter();
        OnWaveEnded?.Invoke();
        OnEnterShop?.Invoke();
    }
    public void DamagePlayer(int dmg)
    {
        health -= dmg;
        OnStatsChanged?.Invoke();
        if (health <= 0)
        {
            gameoverscreen.SetActive(true);
            darkbackground.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        OnStatsChanged?.Invoke();
        return true;
    }

    public void EarnCoins(int amount)
    {
        coins += amount;
        OnStatsChanged?.Invoke();
    }

    public void UpdateWaveCounter()
    {
        if (waveCounterText != null)
            waveCounterText.text = $"Wave {currentWave}";
    }

    public void UpdateEnemiesLeftCounter()
    {
        if (enemiesLeftText != null)
        {
            int enemiesLeft = WaveManager.I.GetAliveEnemyCount();
            enemiesLeftText.text = $"Enemies Left: {enemiesLeft}";
        }
    }
}
