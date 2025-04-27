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

    private LineRenderer lr;

    void Awake()
    {
        // grab or add the LineRenderer
        lr = GetComponent<LineRenderer>();
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
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float ang   = Mathf.Deg2Rad * angleStep * i;
            Vector3 pos = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            lr.SetPosition(i, pos);
        }
    }

    public void Show()   => lr.enabled = true;
    public void Hide()   => lr.enabled = false;
    public void Toggle() => lr.enabled = !lr.enabled;
}
