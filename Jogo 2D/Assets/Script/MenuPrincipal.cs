using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Sair()
    {
        Application.Quit();
    }
}
