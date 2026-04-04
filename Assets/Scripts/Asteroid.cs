using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float radius = 1.5f;
    [HideInInspector] public bool dead;

    const float minSplitRadius = 0.4f;

    void OnCollisionEnter(Collision collision)
    {
        if (dead) return;
        var other = collision.gameObject.GetComponent<Asteroid>();
        if (other == null || other.dead) return;

        dead = true;
        other.dead = true;

        float speed = GetComponent<Rigidbody>().linearVelocity.magnitude;
        SpawnFragments(collision.contacts[0].point, speed);

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    // Called by Projectile when a shot hits this asteroid
    public void Explode()
    {
        if (dead) return;
        dead = true;

        float speed = GetComponent<Rigidbody>().linearVelocity.magnitude;
        SpawnFragments(transform.position, speed);

        Destroy(gameObject);
    }

    void SpawnFragments(Vector3 pos, float parentSpeed)
    {
        if (radius <= minSplitRadius) return;

        float fragmentRadius = radius * 0.5f;
        int count = Random.Range(3, 7);

        for (int i = 0; i < count; i++)
        {
            float speed = Mathf.Max(4f, parentSpeed * Random.Range(1f, 2f));
            AsteroidSpawner.Spawn(pos, fragmentRadius, Random.onUnitSphere * speed);
        }
    }
}
