using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Rendering;  // for SortingGroup
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    public static HandManager I { get; private set; }

    [Header("Card Setup")]
    [Tooltip("This is your generic card prefab used by ReceiveCard()")]
    [SerializeField] private GameObject cardPrefab;
    [Tooltip("All your different card prefabs (with Card component)")]
    [SerializeField] private Card[] cardPrefabs;

    [Header("Hand Layout")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxHandSize = 5;
    [Tooltip("Sorting order for the first card; next cards increment by +1")]
    [SerializeField] private int cardBaseOrder = 20;

    private readonly List<GameObject> cardsInHand = new();

    void Awake()
    {
        I = this;
        if (spawnPoint == null)
            Debug.LogError("HandManager ▶ spawnPoint is NOT assigned!");
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

    // 1) instantiate the specific prefab we passed in
    GameObject card = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    cardsInHand.Add(card);

    // Optional: Update the card's appearance
    var cardUI = card.GetComponent<UICard>();
    if (cardUI != null)
        cardUI.Setup(cardData, null); // no shop needed

    // 2) assign its sorting order
    var sg = card.GetComponent<SortingGroup>();
    if (sg != null)
        sg.sortingOrder = cardBaseOrder + (cardsInHand.Count - 1);

    Debug.LogWarning($"Updating card position, {cardsInHand.Count} cards in hand");

    // 3) re-layout
    UpdateCardPosition();
}


    /// <summary>
    /// Call this to give the user back the exact card they cancelled.
    /// </summary>
    public void ReturnCard(GameObject towerPre, GameObject previewPre)
    {
        foreach (var cp in cardPrefabs)
        {
            if (cp.towerPrefab == towerPre 
             && cp.previewPrefab == previewPre)
            {
                // swap in the correct prefab and re-use ReceiveCard()
                cardPrefab = cp.gameObject;
                //ReceiveCard();
                return;
            }
        }

        Debug.LogWarning($"HandManager: no card prefab found for tower {towerPre.name}");
    }

    private void UpdateCardPosition()
    {
        cardsInHand.RemoveAll(c => c == null);

        if (cardsInHand.Count == 0) return;

        float spacing  = 1f / cardsInHand.Count;
        float firstT   = 0.5f - (cardsInHand.Count - 1) * spacing / 2f;
        var   spline   = splineContainer.Spline;
        var   scTrans  = splineContainer.transform;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float t = firstT + i * spacing;

            // local → world transform
            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = scTrans.TransformPoint(localPos);

            // rotation if desired
            Vector3 localT = spline.EvaluateTangent(t);
            Vector3 localU = spline.EvaluateUpVector(t);
            Vector3 worldF = scTrans.TransformDirection(localT);
            Vector3 worldU = scTrans.TransformDirection(localU);
            Quaternion worldRot = Quaternion.LookRotation(
                worldU,
                Vector3.Cross(worldU, worldF).normalized
            );

            // tween each card  
            var tf = cardsInHand[i].transform;
            tf.DOMove(worldPos, 0.25f).SetEase(Ease.OutQuad);
            tf.DORotateQuaternion(worldRot, 0.25f).SetEase(Ease.OutQuad);
        }
    }
}


// using System.Collections.Generic;
// using System.Numerics;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.Splines;
// using UnityEngine.UI;

// public class HandManager : MonoBehaviour
// {
//     [Tooltip("The UI panel that holds the cards")]
//     public RectTransform handPanel;

//     [Tooltip("Drag your CardPrefab here")]
//     [SerializeField] private GameObject cardPrefab;
//     [SerializeField] private SplineContainer splineContainer;
//     [SerializeField] private Transform HandContainer;
//     [SerializeField] private Transform spawnPoint;
//     [SerializeField] private int maxHandSize;

//     private List<GameObject> cardsInHand = new();

//     void Start()
//     {
//         receiveCard();
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             receiveCard();
//         }
//     }

// void receiveCard(){
//     if (cardsInHand.Count >= maxHandSize) return;

//     // 1) figure out the spawn point in VIEWPORT coords:
//     //    x = .5 is center; y = .1 is 10% up from bottom
//     float t = (cardsInHand.Count + 0.5f) / maxHandSize;
//     UnityEngine.Vector3 vp = new UnityEngine.Vector3(t, 0.1f, 0f);

//     // 2) convert that to world—pick a Z such that the hand sits in front of the level:
//     Camera cam = Camera.main;
//     vp.z = Mathf.Abs(cam.transform.position.z - HandContainer.transform.position.z);
//     UnityEngine.Vector3 worldPos = cam.ViewportToWorldPoint(vp);

//     // 3) instantiate as a child of HandContainer
//     GameObject card = Instantiate(cardPrefab, worldPos, UnityEngine.Quaternion.identity, HandContainer);
//     card.transform.localScale = UnityEngine.Vector3.one * 0.5f; // tweak so it’s a reasonable size
//     cardsInHand.Add(card);

//     // 4) now lay out all cards along your spline if you like:
//     UpdateCardPosition();
// }

// private void UpdateCardPosition(){
//     if (cardsInHand.Count == 0) return;

//     float spread = 1f / maxHandSize;
//     float firstT = 0.5f - (cardsInHand.Count - 1) * spread / 2f;
//     Spline s     = splineContainer.Spline;
//     for (int i = 0; i < cardsInHand.Count; i++){
//         float tt = firstT + i * spread;
//         UnityEngine.Vector3 targetWorld = s.EvaluatePosition(tt);
//         cardsInHand[i].transform
//             .DOMove(targetWorld, 0.25f)
//             .SetEase(Ease.OutQuad);
//         // rotation same as before if needed…
//     }
// }
// }

