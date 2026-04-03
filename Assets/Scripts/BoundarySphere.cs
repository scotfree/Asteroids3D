using UnityEngine;

// Attach to any object (Ship, Asteroid) that should wrap around the play area.
// Both objects must have a Rigidbody.
[RequireComponent(typeof(Rigidbody))]
public class BoundarySphere : MonoBehaviour
{
    public static float Radius = 50f;

    Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        if (rb.position.magnitude <= Radius) return;

        // Teleport to the antipodal point on the sphere surface.
        // Velocity is unchanged — Newtonian wrap.
        rb.position = -rb.position.normalized * Radius;
    }
}
