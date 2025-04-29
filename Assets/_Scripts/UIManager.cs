using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text coinText, healthText;


    void Start()
    {
        if (GameManager.I == null)
        {
            return;
        }

        GameManager.I.OnStatsChanged += UpdateStats;
        UpdateStats();
    }

    void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        int coins = GameManager.I.coins;
        int health = GameManager.I.health;

        Debug.Log($"UIManager: coins={coins}, health={health}");

        if (coinText != null) coinText.text = coins.ToString();
        if (healthText != null) healthText.text = health.ToString();
    }
}
