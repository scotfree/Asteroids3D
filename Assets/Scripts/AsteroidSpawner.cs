using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] int count = 15;
    [SerializeField] float minSpeed = 3f;
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float asteroidRadius = 1.5f;
    [SerializeField] float minSpawnDistance = 20f; // keep asteroids away from ship at start

    void Start()
    {
        for (int i = 0; i < count; i++)
            SpawnAsteroid();
    }

    void SpawnAsteroid()
    {
        // Random position inside boundary sphere, at least minSpawnDistance from origin
        Vector3 pos;
        do { pos = Random.insideUnitSphere * BoundarySphere.Radius; }
        while (pos.magnitude < minSpawnDistance);

        // Random velocity not pointing toward origin:
        // if the random direction heads toward origin, flip it
        Vector3 dir = Random.onUnitSphere;
        if (Vector3.Dot(dir, -pos.normalized) > 0)
            dir = -dir;

        // Build the asteroid GameObject from a Unity primitive sphere
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Asteroid";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * asteroidRadius * 2f;

        // Rocky grey-brown material
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", new Color(0.5f, 0.45f, 0.4f));
        go.GetComponent<Renderer>().material = mat;

        // Physics — add components explicitly so the order is clear
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.linearVelocity = dir * Random.Range(minSpeed, maxSpeed);

        go.AddComponent<BoundarySphere>();
        go.AddComponent<Asteroid>();
    }
}
