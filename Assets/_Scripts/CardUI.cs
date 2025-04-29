using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text costText;
    private Card cardData;
    private CardShop cardShop;

    public void Setup(Card data, CardShop shop)
    {
        cardData = data;
        cardShop = shop;

        backgroundImage.sprite = cardData.artwork;

        costText.text = $"{cardData.cost}";

        Button btn = GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => cardShop.TryBuyCard(cardData, gameObject));
        }
    }

}
