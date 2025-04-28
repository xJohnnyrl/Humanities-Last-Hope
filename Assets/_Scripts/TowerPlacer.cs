using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TowerPlacer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;

    [Header("Tower Prefab")]
    public GameObject towerPrefab;

    [Header("Preview Settings")]
    public GameObject previewPrefab;
    public Color validColor   = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();
    private GameObject previewInstance;
    private SpriteRenderer[] previewRenderers;
    private Vector3Int lastCellPos;
    [HideInInspector] public bool isPlacing = false;

    [HideInInspector] public GameObject lastTowerPrefab;
    [HideInInspector] public GameObject lastPreviewPrefab;


void Start()
{
    if (previewPrefab == null)
    {
        Debug.LogError("👉 previewPrefab not set on TowerPlacer!");
        return;
    }

        previewInstance  = Instantiate(previewPrefab);
        previewRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var c in previewInstance.GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        previewInstance.SetActive(false);
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPlacing = false;
            previewInstance.SetActive(false);
            Debug.Log("👉 Canceling Placement!");
            CancelPlacement();
        }

        if (isPlacing)
        {
            previewInstance.SetActive(true);
            UpdatePreviewPosition();
            if (Input.GetMouseButtonDown(0))
                TryPlaceTower();
        }
    }

    void UpdatePreviewPosition()
    {
        Vector3 wp       = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z             = 0;
        var cellPos      = groundTilemap.WorldToCell(wp);
        var center       = groundTilemap.GetCellCenterWorld(cellPos);

        if (cellPos == lastCellPos) return;
        lastCellPos = cellPos;
        previewInstance.transform.position = center;

        bool onGround   = groundTilemap.HasTile(cellPos);
        bool onPath     = pathTilemap.HasTile(cellPos);
        bool isOcc      = occupied.Contains(cellPos);
        bool valid      = onGround && !onPath && !isOcc;

        // Tint all child renderers
        var tint = valid ? validColor : invalidColor;
        foreach (var sr in previewRenderers)
            sr.color = tint;
    }

    void TryPlaceTower()
    {
        Vector3 wp         = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z               = 0;
        var cellPos        = groundTilemap.WorldToCell(wp);
        if (!groundTilemap.HasTile(cellPos)) return;
        if ( pathTilemap.HasTile(cellPos))   return;
        if ( occupied.Contains(cellPos))     return;

        var spawnPos = groundTilemap.GetCellCenterWorld(cellPos);
        Instantiate(towerPrefab, spawnPos, Quaternion.identity);
        occupied.Add(cellPos);
        isPlacing = false;
        previewInstance.SetActive(false);
    }

    public void StartPlacement(GameObject tower, GameObject preview)
    {
        lastTowerPrefab   = tower;
        lastPreviewPrefab = preview;

        towerPrefab   = tower;
        previewPrefab = preview;
        isPlacing     = true;

        // **if they picked a different preview prefab**, re-spawn it:
        if (previewInstance == null || previewInstance.name.Contains(previewPrefab.name) == false)
        {
            Destroy(previewInstance);
            previewInstance  = Instantiate(previewPrefab);
            previewRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
            foreach (var c in previewInstance.GetComponentsInChildren<Collider2D>())
                c.enabled = false;
        }

        previewInstance.SetActive(true);
    }

    public void CancelPlacement()
    {
        // give the card back
        if (HandManager.I != null && lastTowerPrefab != null)
            HandManager.I.ReturnCard(lastTowerPrefab, lastPreviewPrefab);

        // clear remembered so you don’t return twice
        lastTowerPrefab = lastPreviewPrefab = null;
    }
}
