using UnityEngine;

public class Altar : MonoBehaviour
{
    private Animator animator;
    private bool playerPerto;
    private bool jaInteragiu;

    public GameObject textoInteracao; // Arrasta o teu objeto "Interagir" para aqui no Inspector
    public Vector3 offsetCanvas = new Vector3(0, 80, 0); // Desvio em pixéis para ficar acima do Altar

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
        // Se o jogador estiver perto, atualizar a posição do texto para seguir o Altar no ecrã
        if (playerPerto && !jaInteragiu && textoInteracao != null && textoInteracao.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            textoInteracao.transform.position = screenPos + offsetCanvas;
        }

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
