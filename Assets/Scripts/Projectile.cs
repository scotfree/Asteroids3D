using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        // No wrapping — shots vanish at the boundary
        if (rb.position.magnitude > BoundarySphere.Radius)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        var asteroid = other.GetComponent<Asteroid>();
        if (asteroid == null || asteroid.dead) return;

        asteroid.Explode();
        Destroy(gameObject);
    }
}
