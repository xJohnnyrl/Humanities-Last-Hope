using UnityEngine;
using TMPro;    

public class UIManager : MonoBehaviour
{
    [Header("Drag your UI Text fields here")]    
    public TMP_Text coinText, healthText;


    void Start()
    {
        // sanity checks
        if (coinText   == null) Debug.LogError("UIManager: coinText is NOT assigned!");
        if (healthText == null) Debug.LogError("UIManager: healthText is NOT assigned!");
        if (GameManager.I == null)
        {
            Debug.LogError("UIManager: GameManager.I is still null in Start!");
            return;
        }

        // subscribe now that GameManager.I is valid
        GameManager.I.OnStatsChanged += UpdateStats;
        Debug.Log("UIManager: subscribed to OnStatsChanged");

        // initial draw
        UpdateStats();
    }

    void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        int coins  = GameManager.I.coins;
        int health = GameManager.I.health;

        Debug.Log($"UIManager: UpdateStats() → coins={coins}, health={health}");

        if (coinText   != null) coinText.text   = coins.ToString();
        if (healthText != null) healthText.text = health.ToString();
    }
}
