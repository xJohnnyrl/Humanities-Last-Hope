using UnityEngine;

public enum Rarity { Rare, Epic, Legendary }
public class Card : MonoBehaviour

{
    [Tooltip("Which tower prefab this card will spawn")]
    public GameObject towerPrefab;

    [Tooltip("Which preview prefab to use when placing")]
    public GameObject previewPrefab;
    public string cardName;
    public Rarity rarity;
    public int cost;
    public Sprite artwork;
}