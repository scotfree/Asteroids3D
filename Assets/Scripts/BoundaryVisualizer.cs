using UnityEngine;

// Draws three great circles (one per world axis) to show the boundary sphere.
// Attach to any empty GameObject — no other setup needed.
public class BoundaryVisualizer : MonoBehaviour
{
    [SerializeField] int segments = 64;
    [SerializeField] Color lineColor = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] float lineWidth = 0.3f;

    void Start()
    {
        float r = BoundarySphere.Radius;
        // axis = rotation axis of the circle, startDir = first point direction (must be perpendicular to axis)
        DrawGreatCircle(Vector3.up,      Vector3.right,   r);  // XZ plane (horizontal)
        DrawGreatCircle(Vector3.right,   Vector3.up,      r);  // YZ plane
        DrawGreatCircle(Vector3.forward, Vector3.right,   r);  // XY plane
    }

    void DrawGreatCircle(Vector3 axis, Vector3 startDir, float radius)
    {
        var go = new GameObject($"Circle_{axis}");
        go.transform.parent = transform;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        // Try URP unlit shader; lines will appear pink if the shader name is wrong —
        // in that case create a URP Unlit material in the Project window and assign it here.
        var shader = Shader.Find("Universal Render Pipeline/Unlit")
                  ?? Shader.Find("Unlit/Color");
        var mat = new Material(shader);
        mat.SetColor("_BaseColor", lineColor);
        lr.material = mat;

        // Walk around the circle by rotating startDir around axis
        var step = Quaternion.AngleAxis(360f / segments, axis);
        var point = startDir * radius;
        for (int i = 0; i < segments; i++)
        {
            lr.SetPosition(i, point);
            point = step * point;
        }
    }
}
