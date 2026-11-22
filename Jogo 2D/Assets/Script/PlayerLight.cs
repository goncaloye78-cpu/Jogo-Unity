using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject playerLight;

    private bool nearOrb = false;

    void Start()
    {
        // A luz começa desligada
        playerLight.SetActive(false);
    }

    void Update()
    {
        // Se estiver perto da esfera e carregar E
        if (nearOrb && Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateLight();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LightOrb"))
        {
            nearOrb = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LightOrb"))
        {
            nearOrb = false;
        }
    }

    void ActivateLight()
    {
        // Ativa a luz do jogador
        playerLight.SetActive(true);

        // Destrói a esfera
        GameObject orb = GameObject.FindGameObjectWithTag("LightOrb");

        if (orb != null)
        {
            Destroy(orb);
        }
    }
}
