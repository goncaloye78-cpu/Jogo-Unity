using UnityEngine;

public class Altar : MonoBehaviour
{
    private Animator animator;
    private bool playerPerto;
    private bool jaInteragiu;

    public GameObject textoInteracao;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (textoInteracao != null)
        {
            textoInteracao.SetActive(false);
        }
    }

    void Update()
    {
        if (playerPerto && !jaInteragiu && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("RetirarEspada");
            jaInteragiu = true;

            if (textoInteracao != null)
            {
                textoInteracao.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;

            if (!jaInteragiu && textoInteracao != null)
            {
                textoInteracao.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = false;

            if (textoInteracao != null)
            {
                textoInteracao.SetActive(false);
            }
        }
    }
}
