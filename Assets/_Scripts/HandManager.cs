using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Rendering;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    public static HandManager I { get; private set; }
    [SerializeField] private AudioClip cardReceivedSFX;
    private AudioSource audioSource;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Card[] cardPrefabs;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxHandSize = 5;
    [SerializeField] private int cardBaseOrder = 20;
    private readonly List<GameObject> cardsInHand = new();

    void Awake()
    {
        I = this;
        if (spawnPoint == null)
            Debug.LogError("HandManager spawnPoint is NOT assigned!");


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ReceiveCard(Card cardData, GameObject prefabToSpawn)
    {
        if (cardsInHand.Count >= maxHandSize) return;

        GameObject card = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        cardsInHand.Add(card);

        var cardUI = card.GetComponent<UICard>();
        if (cardUI != null)
            cardUI.Setup(cardData, null);

        var sg = card.GetComponent<SortingGroup>();
        if (sg != null)
            sg.sortingOrder = cardBaseOrder + (cardsInHand.Count - 1);

        Debug.LogWarning($"Updating card position, {cardsInHand.Count} cards in hand");

        UpdateCardPosition();

        if (cardReceivedSFX != null && audioSource != null)
            audioSource.PlayOneShot(cardReceivedSFX);
    }

    public void ReturnCard(GameObject towerPre, GameObject previewPre)
    {
        foreach (var cp in cardPrefabs)
        {
            if (cp.towerPrefab == towerPre
             && cp.previewPrefab == previewPre)
            {
                cardPrefab = cp.gameObject;
                return;
            }
        }

        Debug.LogWarning($"HandManager: no card prefab found for tower {towerPre.name}");
    }

    private void UpdateCardPosition()
    {
        cardsInHand.RemoveAll(c => c == null);

        if (cardsInHand.Count == 0) return;

        float spacing = 1f / cardsInHand.Count;
        float firstT = 0.5f - (cardsInHand.Count - 1) * spacing / 2f;
        var spline = splineContainer.Spline;
        var scTrans = splineContainer.transform;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float t = firstT + i * spacing;

            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = scTrans.TransformPoint(localPos);

            Vector3 localT = spline.EvaluateTangent(t);
            Vector3 localU = spline.EvaluateUpVector(t);
            Vector3 worldF = scTrans.TransformDirection(localT);
            Vector3 worldU = scTrans.TransformDirection(localU);
            Quaternion worldRot = Quaternion.LookRotation(
                worldU,
                Vector3.Cross(worldU, worldF).normalized
            );

            var tf = cardsInHand[i].transform;
            tf.DOMove(worldPos, 0.25f).SetEase(Ease.OutQuad);
            tf.DORotateQuaternion(worldRot, 0.25f).SetEase(Ease.OutQuad);
        }
    }
}