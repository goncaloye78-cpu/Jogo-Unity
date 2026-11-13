using UnityEngine;

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 nextposition;
    private SpriteRenderer spriteRenderer;
    private float previousX;
    public int health = 3; // Podes definir logo aqui ou no inspetor

    private void Start()
    {
        nextposition = pointB.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousX = transform.position.x;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextposition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, nextposition) < 0.25f)
        {
            nextposition = (nextposition == pointB.position)
                ? pointA.position
                : pointB.position;
        }

        if (nextposition.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (nextposition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // CORRIGIDO: Fora do TakeDamage
    void Die()
    {
        Destroy(gameObject);
    }
}