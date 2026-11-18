using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public float attackrange;
    public LayerMask whatIsEnemies;
    public int damage;

    void Update()
    {
        if (timeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.K))
            {
                timeBtwAttack = startTimeBtwAttack;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackrange, whatIsEnemies);

                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    if (enemiesToDamage[i].TryGetComponent(out Enemy enemy))
                    {
                        enemy.TakeDamage(damage);
                    }
                    else if (enemiesToDamage[i].TryGetComponent(out Obstaculo obstaculo))
                    {
                        obstaculo.TakeDamage(damage);
                    }
                }
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackrange);
        }
    }
}
