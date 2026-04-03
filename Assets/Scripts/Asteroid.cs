using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float radius = 1.5f;
    [HideInInspector] public bool dead;

    // Fragments smaller than this are destroyed without splitting further
    const float minSplitRadius = 0.4f;

    void OnCollisionEnter(Collision collision)
    {
        if (dead) return;

        var other = collision.gameObject.GetComponent<Asteroid>();
        if (other == null || other.dead) return;

        // Mark both dead immediately so the other asteroid's OnCollisionEnter skips this
        dead = true;
        other.dead = true;

        // Spawn fragments from the contact point if large enough
        if (radius > minSplitRadius)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            float fragmentRadius = radius * 0.5f;
            float parentSpeed = GetComponent<Rigidbody>().linearVelocity.magnitude;
            int count = Random.Range(3, 7); // 3–6 inclusive

            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Random.onUnitSphere;
                float speed = Mathf.Max(4f, parentSpeed * Random.Range(1f, 2f));
                AsteroidSpawner.Spawn(contactPoint, fragmentRadius, dir * speed);
            }
        }

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
