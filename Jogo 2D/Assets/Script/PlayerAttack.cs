using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float timeBtwAttack;
    public float startTimeBtwAttack; //para n dar pa spamar

    public Transform attackPos;
    public float attackrange;
    public LayerMask whatIsEnemies;
    void update()
    {

        /*
        if(timeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.K))
            {
                timeBtwAttack = startTimeBtwAttack;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackrange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<ActualEnemy>().health -= damage;
                }
            }
            
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        */
    }
}
