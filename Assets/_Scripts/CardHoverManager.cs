using UnityEngine;

public class CardHoverManager : MonoBehaviour
{
    [Tooltip("Only cards must be in this layer")]
    public LayerMask cardLayer;

    CardHover currentHover;

    void Update()
    {
        // Convert mouse to world
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z        = 0f;

        // Find the topmost collider under the cursor
        Collider2D hit = Physics2D.OverlapPoint(wp, cardLayer);

        CardHover hover = hit ? hit.GetComponent<CardHover>() : null;

        // If we moved to a different card, unhover the old and hover the new
        if (hover != currentHover)
        {
            if (currentHover != null)
                currentHover.Unhover();

            if (hover != null)
                hover.Hover();

            currentHover = hover;
        }
    }
}
