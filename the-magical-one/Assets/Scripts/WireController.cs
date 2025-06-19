using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Tooltip("The starting point of the wire (e.g., a transform on your plug model).")]
    public Transform startPoint;

    [Tooltip("The ending point of the wire (e.g., a wall socket).")]
    public Transform endPoint;

    [Tooltip("The control point that defines the curve of the wire.")]
    public Transform controlPoint;

    [Tooltip("The number of points to use to draw the curve. More points = smoother curve.")]
    [Range(2, 100)]
    public int vertexCount = 50;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(startPoint != null && endPoint != null && controlPoint != null)
        {
            DrawQuadraticBezierCurve();
        }
    }

    private void DrawQuadraticBezierCurve()
    {
        lineRenderer.positionCount = vertexCount;
        for(int i = 0; i < vertexCount; i++)
        {
            float t = i / (float) (vertexCount - 1);
            Vector3 point = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
            lineRenderer.SetPosition(i, point);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // The formula for a quadratic Bezier curve
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}