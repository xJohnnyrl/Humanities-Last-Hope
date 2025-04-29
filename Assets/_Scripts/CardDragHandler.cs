using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card), typeof(Collider2D))]
public class CardDragHandler : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card data;
    private Vector3 pointerOffset;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool used = false;
    private static bool initialized = false;
    public Texture2D hoverCursor;
    private bool cursorIsSet = false;
    public Vector2 hoverHotspot = Vector2.zero;
    void Awake()
    {
        data = GetComponent<Card>();

        if (!initialized)
        {
            initialized = true;

            if (FindObjectOfType<EventSystem>() == null)
            {
                var go = new GameObject("EventSystem");
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }

            var cam = Camera.main;
            if (cam != null && cam.GetComponent<Physics2DRaycaster>() == null)
                cam.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (used) return;
        Vector3 wp = Camera.main.ScreenToWorldPoint(e.position);
        wp.z = transform.position.z;
        pointerOffset = transform.position - wp;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (used) return;
        Cursor.SetCursor(hoverCursor, hoverHotspot, CursorMode.Auto);
        cursorIsSet = true;
        originalPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(null, worldPositionStays: true);
    }

    public void OnDrag(PointerEventData e)
    {
        if (used) return;
        Vector3 wp = Camera.main.ScreenToWorldPoint(e.position);
        wp.z = transform.position.z;
        transform.position = wp + pointerOffset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (used) return;
        used = true;
        var placer = FindObjectOfType<TowerPlacer>();
        placer.StartPlacement(data.towerPrefab, data.previewPrefab);
        Destroy(gameObject);
    }
}
