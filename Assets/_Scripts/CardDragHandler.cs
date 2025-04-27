using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Card           data;
    RectTransform  rect;
    CanvasGroup    cg;
    Transform      originalParent;
    Vector2        originalPos;
    bool           used = false;

    void Awake()
    {
        data = GetComponent<Card>();
        rect = GetComponent<RectTransform>();
        cg   = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (used) return;

        originalParent      = transform.parent;
        originalPos         = rect.anchoredPosition;
        transform.SetParent(transform.root, false);
        cg.blocksRaycasts   = false;
    }

    public void OnDrag(PointerEventData e)
    {
        if (used) return;
        rect.anchoredPosition += e.delta / 
            (GetComponentInParent<Canvas>().scaleFactor);
    }

    public void OnEndDrag(PointerEventData e)
    {
        cg.blocksRaycasts = true;
        if (used) { SnapBack(); return; }

        // dropped outside the hand panel?
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                originalParent as RectTransform,
                e.position,
                e.pressEventCamera))
        {
            // kick off placement for THIS card’s tower
            var placer = FindFirstObjectByType<TowerPlacer>();
            placer.StartPlacement(data.towerPrefab, data.previewPrefab);

            used = true;
            Destroy(gameObject);
        }
        else
        {
            SnapBack();
        }
    }

    void SnapBack()
    {
        transform.SetParent(originalParent, false);
        rect.anchoredPosition = originalPos;
    }
}
