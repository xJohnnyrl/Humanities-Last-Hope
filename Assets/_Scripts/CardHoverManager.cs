using UnityEngine;
using System.Linq;

public class CardHoverManager : MonoBehaviour
{
    [Tooltip("Only cards must be on this layer")]
    public LayerMask cardLayer;

    [Tooltip("Cursor to show when hovering over a card")]
    public Texture2D hoverCursor;

    [Tooltip("Hotspot of that cursor (in pixels)")]
    public Vector2   hoverHotspot = Vector2.zero;

    // you can leave this null to revert to default
    private bool     cursorIsSet = false;
    private CardHover currentHover;

    void Update()
    {
        // 1) Raycast at mouse
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z = 0;
        var hits = Physics2D.OverlapPointAll(wp, cardLayer);
        var hovers = hits
            .Select(h => h.GetComponentInParent<CardHover>())
            .Where(ch => ch != null)
            .ToList();

        // 2) Pick the topmost by sortingOrder
        CardHover top = null;
        if (hovers.Count > 0)
            top = hovers
                .OrderByDescending(ch => ch.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder)
                .First();

        // 3) If hover‐target changed, swap Hover/Unhover
        if (top != currentHover)
        {
            if (currentHover != null) currentHover.Unhover();
            if (top         != null) top.Hover();
            currentHover = top;
        }

        // 4) Set or reset the OS cursor
        if (currentHover != null && !cursorIsSet)
        {
            Cursor.SetCursor(hoverCursor, hoverHotspot, CursorMode.Auto);
            cursorIsSet = true;
        }
        else if (currentHover == null && cursorIsSet)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            cursorIsSet = false;
        }
    }
}
