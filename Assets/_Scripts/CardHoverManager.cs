using UnityEngine;
using System.Linq;

public class CardHoverManager : MonoBehaviour
{
    public LayerMask cardLayer;
    public Texture2D hoverCursor;
    public Vector2 hoverHotspot = Vector2.zero;
    private bool cursorIsSet = false;
    private CardHover currentHover;

    void Update()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z = 0;
        var hits = Physics2D.OverlapPointAll(wp, cardLayer);
        var hovers = hits
            .Select(h => h.GetComponentInParent<CardHover>())
            .Where(ch => ch != null)
            .ToList();

        CardHover top = null;
        if (hovers.Count > 0)
            top = hovers
                .OrderByDescending(ch => ch.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder)
                .First();

        if (top != currentHover)
        {
            if (currentHover != null) currentHover.Unhover();
            if (top != null) top.Hover();
            currentHover = top;
        }

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
