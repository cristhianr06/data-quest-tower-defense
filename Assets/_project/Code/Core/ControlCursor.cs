using UnityEngine;

public class ControlCursor : MonoBehaviour
{
    [SerializeField] private bool _cursorVisible = false;
    [SerializeField] private CursorLockMode _cursorLockMode = CursorLockMode.Locked;
    void Start()
    {
        Cursor.visible = _cursorVisible;
        Cursor.lockState = _cursorLockMode;
    }
}
