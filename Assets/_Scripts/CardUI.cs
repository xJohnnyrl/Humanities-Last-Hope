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

        // SET the card artwork onto the background image
        backgroundImage.sprite = cardData.artwork;

        // SET the card cost if you want to show it
        costText.text = cardData.cost.ToString();

        // Make the whole card clickable
        Button btn = GetComponentInChildren<Button>();

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => cardShop.TryBuyCard(cardData, gameObject));
        }
    }
}
