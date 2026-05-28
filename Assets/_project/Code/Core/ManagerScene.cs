using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
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
    
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Reset()
    { 
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Close()
    {
        Debug.Log("Close");
        Application.Quit();
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
