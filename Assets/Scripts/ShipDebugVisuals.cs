using UnityEngine;
using System.Collections.Generic;

// Attach to Ship alongside ShipController.
// Shows three visual aids:
//   Green  — thrust direction (forward axis), length scales with current speed
//   Yellow — current velocity vector, length scales with speed
//   Cyan   — fading trail of the cone nose, showing the arc of rotation
[RequireComponent(typeof(Rigidbody))]
public class ShipDebugVisuals : MonoBehaviour
{
    [SerializeField] float velocityScale  = 1f;   // world-units per unit of speed
    [SerializeField] int   trailLength    = 30;   // how many nose positions to remember
    [SerializeField] float trailInterval  = 0.05f; // seconds between trail samples

    Rigidbody rb;
    LineRenderer forwardLine;
    LineRenderer velocityLine;
    LineRenderer noseLine;

    readonly Queue<Vector3> noseHistory = new Queue<Vector3>();
    float trailTimer;

    // Cone nose sits at local z = +1.5 after centering the mesh on the ship pivot
    Vector3 NoseWorld => transform.TransformPoint(new Vector3(0f, 0f, 1.5f));

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Tapered lines suggest directionality (wide at origin, narrow at tip)
        forwardLine  = MakeLine("ForwardLine",  Color.green,  startWidth: 0.2f, endWidth: 0.04f);
        velocityLine = MakeLine("VelocityLine", Color.yellow, startWidth: 0.2f, endWidth: 0.04f);
        noseLine     = MakeLine("NoseLine",     Color.cyan,   startWidth: 0.08f, endWidth: 0.08f);
        noseLine.positionCount = 0;

        // Trail fades from transparent (oldest) to opaque (newest)
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.cyan, 0f), new GradientColorKey(Color.cyan, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        noseLine.colorGradient = gradient;
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        // Green forward line starts at the nose, extends in thrust direction.
        // Minimum length of 3 so it's visible even at rest.
        float forwardLen = Mathf.Max(3f, speed) * velocityScale;
        SetLine(forwardLine, NoseWorld, NoseWorld + transform.forward * forwardLen);

        // Yellow velocity line starts at ship centre, points along actual momentum.
        SetLine(velocityLine, transform.position, transform.position + rb.linearVelocity * velocityScale);

        // Cyan nose trail — sample nose world position every trailInterval seconds
        trailTimer += Time.deltaTime;
        if (trailTimer >= trailInterval)
        {
            trailTimer = 0f;
            noseHistory.Enqueue(NoseWorld);
            if (noseHistory.Count > trailLength)
                noseHistory.Dequeue();
        }

        var pts = noseHistory.ToArray();
        noseLine.positionCount = pts.Length;
        if (pts.Length > 0) noseLine.SetPositions(pts);
    }

    void SetLine(LineRenderer lr, Vector3 from, Vector3 to)
    {
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
    }

    LineRenderer MakeLine(string name, Color color, float startWidth, float endWidth)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startWidth = startWidth;
        lr.endWidth   = endWidth;
        lr.startColor = color;
        lr.endColor   = color;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", color);
        lr.material = mat;
        return lr;
    }
}
