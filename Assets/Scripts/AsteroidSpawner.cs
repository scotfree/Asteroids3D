using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] int count = 15;
    [SerializeField] float minSpeed = 3f;
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float minAsteroidRadius = 4.5f;
    [SerializeField] float maxAsteroidRadius = 50.0f;
    [SerializeField] float minSpawnDistance = 30f;

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos;
            do { pos = Random.insideUnitSphere * BoundarySphere.Radius; }
            while (pos.magnitude < minSpawnDistance);

            Vector3 dir = Random.onUnitSphere;
            if (Vector3.Dot(dir, -pos.normalized) > 0) dir = -dir;

            float radius = Random.Range(minAsteroidRadius, maxAsteroidRadius);
            Spawn(pos, radius, dir * Random.Range(minSpeed, maxSpeed));
        }
    }

    // Static so Asteroid.cs can call it when splitting
    public static GameObject Spawn(Vector3 pos, float radius, Vector3 velocity)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Asteroid";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * radius * 2f;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", new Color(0.5f, 0.45f, 0.4f));
        go.GetComponent<Renderer>().material = mat;

        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.linearVelocity = velocity;

        var asteroid = go.AddComponent<Asteroid>();
        asteroid.radius = radius;

        go.AddComponent<BoundarySphere>();

        return go;
    }
}
