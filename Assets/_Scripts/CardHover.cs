using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering; // for SortingGroup

[RequireComponent(typeof(Collider2D), typeof(SortingGroup))]
public class CardHover : MonoBehaviour
{
    [Tooltip("How far above its current Y to lift")]
    public float hoverHeight   = 0.5f;
    [Tooltip("Seconds to tween up/down")]
    public float hoverDuration = 0.2f;
    [Tooltip("Temporary sortingOrder on hover")]
    public int   hoverOrder    = 1000;

    float        baseY;
    bool         isLifted    = false;
    SortingGroup sg;
    int          originalOrder;

    void Awake()
    {
        sg            = GetComponent<SortingGroup>();
        originalOrder = sg.sortingOrder;
    }

    /// <summary>
    /// Call this to simulate OnMouseEnter.
    /// </summary>
    public void Hover()
    {
        if (isLifted) return;
        isLifted = true;

        // record current Y (in case we've moved)
        baseY = transform.position.y;

        // stop any tweens
        transform.DOKill();

        // lift
        transform
          .DOMoveY(baseY + hoverHeight, hoverDuration)
          .SetEase(Ease.OutQuad);

        // bring to front
        sg.sortingOrder = hoverOrder;
    }
    public void Unhover()
    {
        if (!isLifted) return;
        isLifted = false;

        transform.DOKill();

        // drop back
        transform
          .DOMoveY(baseY, hoverDuration)
          .SetEase(Ease.OutQuad);

        // restore original layer
        sg.sortingOrder = originalOrder;
    }
}
