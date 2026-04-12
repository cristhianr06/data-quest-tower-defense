using System.Collections.Generic;
using UnityEngine;

namespace VrBand.Assets._model_explorer_vr.Code.Tools
{
    [ExecuteInEditMode]
    public class GizmoVisualizeColliders : MonoBehaviour
    {
        [SerializeField] private Color _colorGizmos;

        public Color ColorGizmos
        {
            get => _colorGizmos;
            set => _colorGizmos = value;
        }

        private void Awake()
        {
            // hide this component in the inspector
#if UNITY_EDITOR
            hideFlags = HideFlags.HideInInspector;
#endif
        }
        private void OnDrawGizmos()
        {
            // Get all colliders of the object
#if UNITY_EDITOR
            RefreshDrawGizmos();
#endif
        }
        private void RefreshDrawGizmos()
        {
            var collidersObj = new List<Collider>(GetComponents<Collider>());
            if (collidersObj.Count == 0) return;
            foreach (var colliderObj in collidersObj)
            {
                Gizmos.color = _colorGizmos;

                // Guardar el estado actual de la matriz del Gizmo
                Gizmos.matrix = colliderObj.transform.localToWorldMatrix;

                if (colliderObj is BoxCollider boxCollider)
                {
                    var boxColliderPos = boxCollider.center;
                    Gizmos.DrawWireCube(boxColliderPos, boxCollider.size);
                }
                else if (colliderObj is SphereCollider sphereCollider)
                {
                    var sphereColliderPos = sphereCollider.center;
                    Gizmos.DrawWireSphere(sphereColliderPos, sphereCollider.radius);
                }

                // Restaurar el estado de la matriz del Gizmo
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}