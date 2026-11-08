using UnityEngine;
using UnityEngine.EventSystems;

public class FixMenu : MonoBehaviour
{
    public GameObject targetButton;

    void Start()
    {
        SelecionarBotao();
    }

    // Se voltas à cena e o Unity perde o foco, isto força novamente
    void OnEnable()
    {
        SelecionarBotao();
    }

    void SelecionarBotao()
    {
        if (targetButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(targetButton);
        }
    }
}
