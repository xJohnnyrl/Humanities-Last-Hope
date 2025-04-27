using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [Tooltip("How many segments to use for the circle")]
    public int segments = 60;
    [Tooltip("Width of the circle line in world units")]
    public float lineWidth = 0.05f;
    [Tooltip("Color and alpha of the ring")]
    public Color color = new Color(0f, 0f, 0f, 0.5f);

    [HideInInspector] public float radius = 3f;

    [Tooltip("Optional: if you want the circle to center somewhere other than this transform's pivot, drop a child here.")]
    public Transform rangeOrigin;

    private LineRenderer lr;
    private Vector3      centerOffset;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        centerOffset = rangeOrigin != null 
            ? rangeOrigin.localPosition 
            : Vector3.zero;

        lr.useWorldSpace = false;    // so positions are local to the tower
        lr.loop          = true;
        lr.positionCount = segments + 1;
        lr.startWidth    = lr.endWidth = lineWidth;
        lr.startColor    = lr.endColor = color;
        lr.material      = new Material(Shader.Find("Sprites/Default"));

        DrawCircle();
        lr.enabled = false;          // hidden by default
    }

    public void DrawCircle()
    {
        if (lr == null) return;

        float step = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.Deg2Rad * step * i;
            Vector3 p = new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            lr.SetPosition(i, p + centerOffset);
        }
    }

    public void Show()   => lr.enabled = true;
    public void Hide()   => lr.enabled = false;
    public void Toggle() => lr.enabled = !lr.enabled;
}
