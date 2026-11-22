using UnityEngine;
using UnityEngine.SceneManagement;

/*
public class PlayerPersiste : MonoBehaviour
{
    private static PlayerPersiste instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Mantém o jogador entre cenas
        DontDestroyOnLoad(gameObject);
    }
}
*/
public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string targetID = SceneTransitionData.spawnID;

        if (string.IsNullOrEmpty(targetID))
            return;

        SpawnPoint[] spawnPoints = Object.FindObjectsByType<SpawnPoint>();

        foreach (SpawnPoint point in spawnPoints)
        {
            if (point.spawnID == targetID)
            {
                transform.position = point.transform.position;

                // Limpa a velocidade do jogador
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                SceneTransitionData.spawnID = "";

                Debug.Log("Jogador colocado no SpawnPoint: " + targetID);
                return;
            }
        }

        Debug.LogWarning("Não foi encontrado o SpawnPoint: " + targetID);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
