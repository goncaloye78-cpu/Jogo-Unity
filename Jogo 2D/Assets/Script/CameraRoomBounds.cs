using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRoomBounds : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        AtualizarLimites();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarLimites();
    }

    private void AtualizarLimites()
    {
        GameObject limites = GameObject.Find("Camera_Limites");

        if (limites == null)
        {
            Debug.LogWarning("Camera_Limites não encontrado nesta sala!");
            return;
        }

        Collider2D collider = limites.GetComponent<Collider2D>();

        if (collider == null)
        {
            Debug.LogWarning("Camera_Limites não tem Collider2D!");
            return;
        }

        confiner.BoundingShape2D = collider;
        confiner.InvalidateBoundingShapeCache();

        Debug.Log("Limites da câmara atualizados!");
    }
}
