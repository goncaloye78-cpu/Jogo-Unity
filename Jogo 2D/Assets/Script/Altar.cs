using UnityEngine;

public class Altar : MonoBehaviour
{
    private Animator animator;
    private bool playerPerto;
    private bool jaInteragiu =false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerPerto && !jaInteragiu && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("RetirarEspada");
            jaInteragiu = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = false;
        }
    }
}
