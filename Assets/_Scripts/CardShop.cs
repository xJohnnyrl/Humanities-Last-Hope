using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardShop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Card[] cardPrefabs;
    [SerializeField] private GameObject uiCardPrefab; 
    [SerializeField] private Transform cardHolder;
    [SerializeField] private TMP_Text playerCoinsText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;
    [SerializeField] private HandManager handManager;

    [Header("Settings")]
    [SerializeField] private int refreshCost = 5;
    [SerializeField] private int cardsToSpawn = 3;

    private List<GameObject> cardsInShop = new();
    private Card selectedCard;
    private GameObject selectedCardGO;

    private void Start()
    {
        refreshButton.onClick.AddListener(RefreshShop);
        confirmPanel.SetActive(false);
        RefreshShop();
        UpdatePlayerCoinsUI();
        GameManager.I.OnStatsChanged += UpdatePlayerCoinsUI;
    }

    private void OnDestroy()
    {
        GameManager.I.OnStatsChanged -= UpdatePlayerCoinsUI;
    }

    private void RefreshShop()
    {
        if (!GameManager.I.SpendCoins(refreshCost))
        {
            Debug.Log("Not enough coins to refresh!");
            return;
        }

        UpdatePlayerCoinsUI();
        ClearCards();
        SpawnCards();
    }

private void SpawnCards()
{
    // 1. Make a copy of the available cards
    List<Card> availableCards = new List<Card>(cardPrefabs);

    // 2. Shuffle them
    for (int i = 0; i < availableCards.Count; i++)
    {
        int randomIndex = Random.Range(i, availableCards.Count);
        (availableCards[i], availableCards[randomIndex]) = (availableCards[randomIndex], availableCards[i]);
    }

    // 3. Take the first 'cardsToSpawn' cards
    for (int i = 0; i < cardsToSpawn && i < availableCards.Count; i++)
    {
        Card randomCard = availableCards[i];

        GameObject uiCard = Instantiate(uiCardPrefab, cardHolder);
        cardsInShop.Add(uiCard);

        UICard uiCardComponent = uiCard.GetComponent<UICard>();
        if (uiCardComponent != null)
        {
            uiCardComponent.Setup(randomCard, this);
        }
    }
}


    private void ClearCards()
    {
        foreach (var card in cardsInShop)
        {
            Destroy(card);
        }
        cardsInShop.Clear();
    }

    public void TryBuyCard(Card card, GameObject cardGO)
    {
        if (GameManager.I.coins < card.cost)
        {
            Debug.Log("Not enough coins to buy!");
            return;
        }

        selectedCard = card;
        selectedCardGO = cardGO;

        confirmPanel.SetActive(true);
        confirmText.text = $"Buy {card.cardName} for {card.cost} coins?";

        confirmYesButton.onClick.RemoveAllListeners();
        confirmNoButton.onClick.RemoveAllListeners();

        confirmYesButton.onClick.AddListener(ConfirmBuy);
        confirmNoButton.onClick.AddListener(CancelBuy);
        Debug.Log("Card has been clicked!");
    }

    private void ConfirmBuy()
    {
        if (!GameManager.I.SpendCoins(selectedCard.cost))
        {
            Debug.Log("Failed to spend coins");
            return;
        }

        UpdatePlayerCoinsUI();
        confirmPanel.SetActive(false);

        // Add the card to the hand manager
        handManager.ReceiveCard(selectedCard);

        // Remove the card from the shop
        Destroy(selectedCardGO);
        cardsInShop.Remove(selectedCardGO);
    }

    private void CancelBuy()
    {
        confirmPanel.SetActive(false);
    }

    private void UpdatePlayerCoinsUI()
    {
        playerCoinsText.text = GameManager.I.coins.ToString();
    }
}
