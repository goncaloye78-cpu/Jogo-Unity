using UnityEngine;

public class Altar : MonoBehaviour
{
    private Animator animator;
    private Interactable interactable;
    private bool playerPerto;
    private bool jaInteragiu;

    void Start()
    {
        animator = GetComponent<Animator>();
        interactable = GetComponent<Interactable>();
    }

    void Update()
    {
        if (playerPerto && !jaInteragiu && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("RetirarEspada");
            jaInteragiu = true;

            // Encontra o Player na cena e ativa a espada dele
            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.temEspada = true;
            }

            if (interactable != null)
            {
                interactable.EsconderETravar();
            }
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