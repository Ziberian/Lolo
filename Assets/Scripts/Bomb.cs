using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public ParticleSystem exp;
    Rigidbody2D rigidbody2d;
    
    public float delay = 2.5f;
    float countdown;
    bool hasExploded = false;

    public float radius = 3.0f;
    public float explosionPower = 8.0f;

    void Awake()
    {
        countdown = delay;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded)
        {
            return;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null && rb.tag != "Player" && hit.gameObject.transform != transform)
            {
                Vector2 direction = (rb.transform.position - transform.position).normalized;
                direction.y += 1.0f;  
                rb.AddForce(direction * explosionPower, ForceMode2D.Impulse);
                if (rb.tag == "Bomb")
                {
                    hasExploded = true;
                    hit.GetComponent<Bomb>().Explode();
                }
            }
        }

        ParticleSystem explosionEffect = Instantiate(exp, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Launch(Vector2 direction, float force)
    {
        direction.y = 2 * direction.y;
        rigidbody2d.AddForce(direction * force);
    }
}
