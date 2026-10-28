using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para carregar cenas

public class SceneControler : MonoBehaviour
{
    [Tooltip("Nome exato da cena para a qual você quer ir")]
    [SerializeField] private string nomeDaProximaCena;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que encostou é o Player (baseado na Tag "Player")
        if (collision.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(nomeDaProximaCena))
            {
                SceneManager.LoadScene(nomeDaProximaCena);
            }
            else
            {
                Debug.LogWarning("O nome da próxima cena não foi definido no Inspetor!");
            }
        }
    }
}
