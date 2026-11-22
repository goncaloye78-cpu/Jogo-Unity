using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public GameObject deathPanel;
    public GameObject restartButton;
    public CanvasGroup canvasGroup;

    public float fadeDuration = 1.5f;

    public void MostrarMorte()
    {
        deathPanel.SetActive(true);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Time.timeScale = 0f;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, tempo / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }

    public void Recomeçar()
    {
        Time.timeScale = 1f;
        StartCoroutine(EliminarPlayerECarregar("GameScene"));
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(EliminarPlayerECarregar("Menu"));
    }

    private IEnumerator EliminarPlayerECarregar(string cena)
    {
        SceneTransitionData.spawnID = "";

        PlayerPersistence player =
            FindAnyObjectByType<PlayerPersistence>();

        if (player != null)
        {
            Destroy(player.gameObject);
        }

        // Espera que o Player antigo seja destruído
        yield return null;

        SceneManager.LoadScene(cena);
    }

    /*

    public void Recomeçar()
    {
        /*
        // Reativar o tempo
        Time.timeScale = 1f;

        // Parar corrotinas
        StopAllCoroutines();

        // Destruir o Player antigo
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Destroy(player);
        }

        // Limpar dados de transição
        SceneTransitionData.spawnID = "";

        // Carregar a primeira fase
        SceneManager.LoadScene("GameScene");
        */

    /*
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    Time.timeScale = 1f;

    StopAllCoroutines();

    deathPanel.SetActive(false);

    canvasGroup.alpha = 0f;
    canvasGroup.interactable = false;
    canvasGroup.blocksRaycasts = false;

    // Definir o Spawn Point inicial da Fase 1
    SceneTransitionData.spawnID = "SP_Inicial";

    // Carregar sempre a primeira fase
    SceneManager.LoadScene("GameScene");

}

public void VoltarAoMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("Menu");
}
*/
}
