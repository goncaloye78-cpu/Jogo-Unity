using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{

    /*
    [Header("Painéis")]
    public GameObject mainMenuPanel;
    public GameObject tutorialPanel;
    public GameObject creditsPanel;

    [Header("Botões")]
    public GameObject jogarButton;
    public GameObject tutorialButton;
    public GameObject creditsButton;   

    public Button tutorialBackButton;
    public Button creditsBackButton;

    public void Jogar()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }

    public void OpenTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        StartCoroutine(SelecionarBotao(tutorialBackButton));
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);

        StartCoroutine(SelecionarBotao(creditsBackButton));
    }

    IEnumerator SelecionarBotao(Button botao)
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botao.gameObject);
        botao.Select();
    }

    public void BackToMenu()
    {
        tutorialPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(tutorialButton);
    }

    [SerializeField] private string menuSceneName;

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
    */
    [Header("Painéis")]
    public GameObject mainMenuPanel;
    public GameObject tutorialPanel;
    public GameObject creditsPanel;

    [Header("Canvas Groups")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup tutorialGroup;
    public CanvasGroup creditsGroup;

    [Header("Botões")]
    public GameObject jogarButton;
    public GameObject tutorialButton;
    public GameObject creditsButton;

    public Button tutorialBackButton;
    public Button creditsBackButton;

    public float fadeDuration = 1.5f;

    void Start()
    {      
        if (mainMenuGroup != null)
        {
            StartCoroutine(FadeIn(mainMenuGroup));
        }

        StartCoroutine(SelecionarBotaoInicial());
    }

    IEnumerator SelecionarBotaoInicial()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(jogarButton);
    }

    public void Jogar()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Sair()
    {
        Application.Quit();
    }

    public void OpenTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        StartCoroutine(FadeIn(tutorialGroup));
        StartCoroutine(SelecionarBotao(tutorialBackButton));
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);

        StartCoroutine(FadeIn(creditsGroup));
        StartCoroutine(SelecionarBotao(creditsBackButton));
    }

    IEnumerator SelecionarBotao(Button botao)
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botao.gameObject);
        botao.Select();
    }

    public void BackToMenu()
    {
        tutorialPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        StartCoroutine(FadeIn(mainMenuGroup));

        StartCoroutine(SelecionarBotao(tutorialButton.GetComponent<Button>()));
    }

    IEnumerator FadeIn(CanvasGroup grupo)
    {
        grupo.alpha = 0f;

        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.unscaledDeltaTime;
            grupo.alpha = Mathf.Lerp(0f, 1f, tempo / fadeDuration);

            yield return null;
        }

        grupo.alpha = 1f;
    }

    [SerializeField] private string menuSceneName;

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
