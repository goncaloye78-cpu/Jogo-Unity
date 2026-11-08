using UnityEngine;

public class MouseHide : MonoBehaviour
{
    void Start()
    {
        // Oculta o cursor apenas no Menu Principal
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
