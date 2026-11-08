using UnityEngine;
using UnityEngine.EventSystems;

public class Menu_Paineis : MonoBehaviour
{
    public GameObject backButton; // Arrasta aqui o botão de Voltar

    void Start()
    {
        // Limpa a seleção atual e força o foco no botão de Voltar
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton);
    }
}
