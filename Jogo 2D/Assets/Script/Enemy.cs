using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 nextposition;
    private SpriteRenderer spriteRenderer;
    private float previousX;
    public int health = 3;

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
        StartCoroutine(BlinkRed());

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Destroy(gameObject);
    }
    
}
