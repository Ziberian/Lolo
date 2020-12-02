using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Animator animator;

    Rigidbody2D rigidbody2d;
    public float delay = 3.0f;

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
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null && rb.tag != "Player")
            {
                Vector2 direction = (rb.transform.position - transform.position).normalized;
                direction.y += 1.0f;
                Debug.Log(direction);   
                rb.AddForce(direction * explosionPower, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
        animator.SetTrigger("Kaboom");
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
}
