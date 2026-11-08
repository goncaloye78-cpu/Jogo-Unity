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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
