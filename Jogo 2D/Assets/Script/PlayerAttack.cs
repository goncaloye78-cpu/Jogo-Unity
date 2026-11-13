using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float timeBtwAttack;
    public float startTimeBtwAttack; // para n dar pa spamar

    public Transform attackPos;
    public float attackrange;
    public LayerMask whatIsEnemies;
    public int damage;

    // CORRIGIDO: 'Update' com U maiúsculo
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
                    enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                }
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    // CORRIGIDO: Retirado de dentro do Update para fora
    void OnDrawGizmosSelected()
    {
        if (attackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackrange);
        }
    }
}
