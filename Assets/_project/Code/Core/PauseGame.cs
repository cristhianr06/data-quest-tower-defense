using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject menuPause;
    private bool _ispaused;
    private InputsPlayer _inputs;
    private void Awake()
    {
        _inputs = new InputsPlayer();
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.Pause.performed += Pause;
    }
    private void Pause(InputAction.CallbackContext context)
    {
        _ispaused = !_ispaused;
        if (_ispaused)
        {
            Time.timeScale = 0;
            menuPause.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            menuPause.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        _inputs.Player.Pause.performed -= Pause;

        _inputs.Disable();
    }
}
