using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    public int health = 2;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteDanificado;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health == 1 && spriteRenderer != null && spriteDanificado != null)
        {
            spriteRenderer.sprite = spriteDanificado;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
