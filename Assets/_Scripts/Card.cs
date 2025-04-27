using UnityEngine;

public class Card : MonoBehaviour
{
    [Tooltip("Which tower prefab this card will spawn")]
    public GameObject towerPrefab;

    [Tooltip("Which preview prefab to use when placing")]
    public GameObject previewPrefab;
}
