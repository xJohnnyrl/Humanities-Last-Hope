using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [Tooltip("The UI panel that holds the cards")]
    public RectTransform handPanel;

    [Tooltip("Drag your CardPrefab here")]
    public Card[] cardPrefabs;

    void Start()
    {
        foreach (var card in cardPrefabs)
        {
            Instantiate(card, handPanel);
        }
    }
}
