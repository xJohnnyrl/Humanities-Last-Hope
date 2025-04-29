using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider2D), typeof(SortingGroup))]
public class CardHover : MonoBehaviour
{
  public float hoverHeight = 0.5f;
  public float hoverDuration = 0.2f;
  public int hoverOrder = 1000;
  float baseY;
  bool isLifted = false;
  SortingGroup sg;
  int originalOrder;

  void Awake()
  {
    sg = GetComponent<SortingGroup>();
    originalOrder = sg.sortingOrder;
  }

  public void Hover()
  {
    if (isLifted) return;
    isLifted = true;

    baseY = transform.position.y;

    transform.DOKill();

    transform
      .DOMoveY(baseY + hoverHeight, hoverDuration)
      .SetEase(Ease.OutQuad);

    sg.sortingOrder = hoverOrder;
  }
  public void Unhover()
  {
    if (!isLifted) return;
    isLifted = false;

    transform.DOKill();

    transform
      .DOMoveY(baseY, hoverDuration)
      .SetEase(Ease.OutQuad);

    sg.sortingOrder = originalOrder;
  }
}
