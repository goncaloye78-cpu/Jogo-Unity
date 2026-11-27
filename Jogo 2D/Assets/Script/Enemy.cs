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
    [Header("Knockback")]
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.15f;

    private bool isKnockedBack;
    private Coroutine knockbackCoroutine;

    private void Start()
    {
        nextposition = pointB.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousX = transform.position.x;
    }

    private void Update()
    {
        if (isKnockedBack) return;
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

    public void TakeDamage(int damage, Vector3? attackerPos = null)
    {
        health -= damage;

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        knockbackCoroutine = StartCoroutine(Knockback(attackerPos));

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator Knockback(Vector3? attackerPos)
    {
        isKnockedBack = true;

        float dirX;
        if (attackerPos.HasValue)
        {
            dirX = transform.position.x >= attackerPos.Value.x ? 1f : -1f;
        }
        else
        {
            dirX = spriteRenderer != null && spriteRenderer.flipX ? 1f : -1f;
        }

        float timer = 0f;
        while (timer < knockbackDuration)
        {
            transform.position += new Vector3(dirX * knockbackForce * Time.deltaTime, 0f, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
        knockbackCoroutine = null;
    }
    void Die()
    {
        Destroy(gameObject);
    }
    
}
