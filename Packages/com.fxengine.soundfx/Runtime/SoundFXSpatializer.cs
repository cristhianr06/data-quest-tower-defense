using UnityEngine;

namespace FXEngine.SoundFX
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    class SoundFXSpatializer : MonoBehaviour
    {
        private Transform _followTarget;

        private void Update()
        {
            Follow(_followTarget);
        }

        private void Follow(Transform target)
        {
            if (target != null)
            {
                transform.SetPositionAndRotation(target.position, target.rotation);
            }
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            Follow(_followTarget);
        }
    }
}
