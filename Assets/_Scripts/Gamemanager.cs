using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameoverscreen;
    [SerializeField] GameObject darkbackground;
    public static GameManager I { get; private set; }

    [Header("Player Stats")]
    public int coins = 100;
    public int health = 10;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public float spawnInterval = 10f;

    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action OnEnterShop;
    public event Action OnStatsChanged;

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
        OnWaveStarted?.Invoke();
        StartCoroutine(WaveManager.I.SpawnWave(
            enemyCount: 5 + currentWave * 2,
            enemySpeed: 2f + currentWave * 0.2f,
            spawnInterval: spawnInterval
        ));
    }

    public void WaveComplete()
    {
        OnWaveEnded?.Invoke();
        OnEnterShop?.Invoke();
    }

    public void DamagePlayer(int dmg)
    {
        health -= dmg;
        OnStatsChanged?.Invoke();
        if (health <= 0) {
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

    public void EarnCoins(int amount){
        coins += amount;
        OnStatsChanged?.Invoke();
    }
}
