using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;    // for Physics2DRaycaster

[RequireComponent(typeof(Card), typeof(Collider2D))]
public class CardDragHandler : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card      data;
    private Vector3   pointerOffset;
    private Vector3   originalPosition;
    private Transform originalParent;
    private bool      used = false;
    private static bool initialized = false;
    [Tooltip("Cursor to show when hovering over a card")]
    public Texture2D hoverCursor;
    private bool     cursorIsSet = false;
        [Tooltip("Hotspot of that cursor (in pixels)")]
    public Vector2   hoverHotspot = Vector2.zero;
    void Awake()
    {
        data = GetComponent<Card>();

        // ── one-time bootstrapping of EventSystem + Physics2DRaycaster ──
        if (!initialized)
        {
            initialized = true;

            // ensure an EventSystem exists
            if (FindObjectOfType<EventSystem>() == null)
            {
                var go = new GameObject("EventSystem");
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }

            // ensure main camera has a Physics2DRaycaster
            var cam = Camera.main;
            if (cam != null && cam.GetComponent<Physics2DRaycaster>() == null)
                cam.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    // 1) record the grab offset in world-space
    public void OnPointerDown(PointerEventData e)
    {
        if (used) return;
        Vector3 wp = Camera.main.ScreenToWorldPoint(e.position);
        wp.z = transform.position.z;
        pointerOffset = transform.position - wp;
    }

    // 2) pick it up
    public void OnBeginDrag(PointerEventData e)
    {
        if (used) return;
        Cursor.SetCursor(hoverCursor, hoverHotspot, CursorMode.Auto);
        cursorIsSet = true;
        originalPosition = transform.position;
        originalParent   = transform.parent;
        // optional: reparent to root so it renders on top
        transform.SetParent(null, worldPositionStays: true);
    }

    // 3) drag it around with the mouse
    public void OnDrag(PointerEventData e)
    {
        if (used) return;
        Vector3 wp = Camera.main.ScreenToWorldPoint(e.position);
        wp.z = transform.position.z;
        transform.position = wp + pointerOffset;
    }

    // 4) on release, place a tower and destroy this card
    public void OnEndDrag(PointerEventData e)
    {
        if (used) return;
        used = true;
        // start tower placement for *this* card’s tower
        var placer = FindObjectOfType<TowerPlacer>();
        placer.StartPlacement(data.towerPrefab, data.previewPrefab);
        Destroy(gameObject);
    }
}
