using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject textoInteracao;
    public Vector3 offsetCanvas = new Vector3(0, 80, 0);

    private bool playerPerto;
    private bool bloqueado;

    void Start()
    {
        if (textoInteracao != null)
        {
            textoInteracao.SetActive(false);
        }
    }

    void Update()
    {
        if (playerPerto && !bloqueado && textoInteracao != null && textoInteracao.activeSelf)
        {
            if (Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                textoInteracao.transform.position = screenPos + offsetCanvas;
            }
        }
    }

    public void EsconderETravar()
    {
        bloqueado = true;
        if (textoInteracao != null)
        {
            textoInteracao.SetActive(false);
        }
    }

    public void Destravar()
    {
        bloqueado = false;
        if (playerPerto && textoInteracao != null)
        {
            textoInteracao.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;

            if (!bloqueado && textoInteracao != null)
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