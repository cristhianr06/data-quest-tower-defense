using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] private int targetFPS = 30;

    private void Awake()
    {
        // Desactiva VSync para que Application.targetFrameRate funcione
        QualitySettings.vSyncCount = 0;

        // Limita los FPS
        Application.targetFrameRate = targetFPS;
    }
}