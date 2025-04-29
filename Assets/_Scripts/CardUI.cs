using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [SerializeField] private Image backgroundImage; // <-- THIS is the big white box
    [SerializeField] private TMP_Text costText;      // <-- (Optional) just shows coin cost

    private Card cardData;
    private CardShop cardShop;

    public void Setup(Card data, CardShop shop)
{
    cardData = data;
    cardShop = shop;

    backgroundImage.sprite = cardData.artwork;

    // Show the card's cost
    costText.text = $"{cardData.cost}"; // Add a coin emoji or leave it just as a number

    // Make the card clickable
    Button btn = GetComponentInChildren<Button>();
    if (btn != null)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => cardShop.TryBuyCard(cardData, gameObject));
    }
}

}
