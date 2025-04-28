using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.Rendering; 

public class HandManager : MonoBehaviour
{
    [Tooltip("Drag your CardPrefab here")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxHandSize;
    [SerializeField] private int cardBaseOrder = 20;

    private List<GameObject> cardsInHand = new();

    void Start()
    {
        receiveCard();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            receiveCard();
        }
    }

    void receiveCard(){
        if (cardsInHand.Count >= maxHandSize) return;

        GameObject card = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        cardsInHand.Add(card);
        var sg = card.GetComponent<SortingGroup>();
        if (sg != null)
            sg.sortingOrder = cardBaseOrder + (cardsInHand.Count - 1);
        UpdateCardPosition();
    }

private void UpdateCardPosition()
{
    if (cardsInHand.Count == 0) return;

    float cardSpacing = 1f / cardsInHand.Count;
    float firstT      = 0.5f - (cardsInHand.Count - 1) * cardSpacing / 2f;
    Spline spline     = splineContainer.Spline;
    Transform scTrans = splineContainer.transform;

    for (int i = 0; i < cardsInHand.Count; i++)
    {
        float t = firstT + i * cardSpacing;

        // 1) get the local-space position on the spline
        UnityEngine.Vector3 localPos = spline.EvaluatePosition(t);
        // 2) convert that to world-space
        UnityEngine.Vector3 worldPos = scTrans.TransformPoint(localPos);

        // 3) likewise for rotation if you care
        UnityEngine.Vector3 localTangent = spline.EvaluateTangent(t);
        UnityEngine.Vector3 localUp      = spline.EvaluateUpVector(t);
        UnityEngine.Vector3 worldForward = scTrans.TransformDirection(localTangent);
        UnityEngine.Vector3 worldUp      = scTrans.TransformDirection(localUp);
        UnityEngine.Quaternion worldRot  = UnityEngine.Quaternion.LookRotation(
                                  worldUp,
                                  UnityEngine.Vector3.Cross(worldUp, worldForward).normalized
                              );

        // 4) tween the card into the correct world position
        cardsInHand[i].transform
           .DOMove(worldPos, 0.25f)
           .SetEase(Ease.OutQuad);
        cardsInHand[i].transform
           .DORotateQuaternion(worldRot, 0.25f)
           .SetEase(Ease.OutQuad);
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

