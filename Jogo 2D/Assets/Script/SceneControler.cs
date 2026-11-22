using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para carregar cenas

/*
public class SceneControler : MonoBehaviour
{
    [Tooltip("Nome exato da cena para a qual você quer ir")]
    [SerializeField] private string nomeDaProximaCena;

    // public static SceneControler Instance;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    
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
*/

public class SceneControler : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string nomeDaProximaCena;

    [SerializeField] private string spawnPointDestino;

    private bool jaTransicionou = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (jaTransicionou || !collision.CompareTag("Player"))
            return;

        if (string.IsNullOrEmpty(nomeDaProximaCena) ||
            string.IsNullOrEmpty(spawnPointDestino))
        {
            Debug.LogWarning("Define a cena e o Spawn Point no Inspector!");
            return;
        }

        jaTransicionou = true;

        // Guarda o ponto onde o jogador deve aparecer
        SceneTransitionData.spawnID = spawnPointDestino;

        // Carrega a nova cena
        SceneManager.LoadScene(nomeDaProximaCena);
    }
}

public static class SceneTransitionData
{
    public static string spawnID;
}

