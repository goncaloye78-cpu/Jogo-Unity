using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class MenuButtonFocus : MonoBehaviour
{
    public GameObject firstButton;

    void OnEnable()
    {
        // Garante que o foco é aplicado mesmo ao voltar à cena
        StartCoroutine(SetFocus());
    }

    IEnumerator SetFocus()
    {
        // Espera um frame para garantir que o EventSystem já iniciou na cena
        yield return null;

        if (EventSystem.current != null && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }
}
