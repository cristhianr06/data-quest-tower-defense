using UnityEngine;

public class RotateCanvas : MonoBehaviour
{
    public bool useStaticCamera = false;

    // Cache de la cámara para evitar el costo de Camera.main en el Update
    private Transform _mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró una Main Camera en la escena.");
            enabled = false;
        }
    }

    // Usamos LateUpdate para asegurar que la cámara ya se haya movido en ese frame
    void LateUpdate()
    {
        if (_mainCameraTransform == null) return;

        if (useStaticCamera)
        {
            // Opción A: Alineación perfecta con el plano de la cámara (ideal para UI)
            transform.rotation = _mainCameraTransform.rotation;
        }
        else
        {
            // Opción B: Mirar directamente a la posición de la cámara
            // Útil si quieres un efecto de perspectiva más marcado
            transform.LookAt(transform.position + _mainCameraTransform.forward);
        }
    }
}