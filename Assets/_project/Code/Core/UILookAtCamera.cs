using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void LateUpdate()
    {
        if (mainCamera == null) return;
        
        // Calcular dirección hacia la cámara (sin restringir eje Y)
        Vector3 direction = mainCamera.transform.position - transform.position;
        
        if (direction != Vector3.zero)
        {
            // Rotación completa en X, Y y Z para apuntar exactamente a la cámara
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}