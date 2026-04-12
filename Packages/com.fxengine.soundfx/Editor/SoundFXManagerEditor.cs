using UnityEditor;

namespace FXEngine.SoundFX.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundFXManager))]
    public class SoundFXManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _preLoadCountProperty;
        private SerializedProperty _capacityModeProperty;
        private SerializedProperty _capacityProperty;

        private void OnEnable()
        {
            _preLoadCountProperty = serializedObject.FindProperty("_preLoadCount");
            _capacityModeProperty = serializedObject.FindProperty("_capacityMode");
            _capacityProperty = serializedObject.FindProperty("_capacity");

            SoundFXManager.OnPoolUpdated += OnPoolUpdated;
        }

        private void OnDisable()
        {
            SoundFXManager.OnPoolUpdated -= OnPoolUpdated;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(_preLoadCountProperty);
            EditorGUILayout.PropertyField(_capacityModeProperty);

            if (_capacityModeProperty.hasMultipleDifferentValues == false)
            {
                var capacityMode = (SoundFXManager.CapacityType)_capacityModeProperty.enumValueIndex;
                if (capacityMode != SoundFXManager.CapacityType.Flexible)
                {
                    EditorGUILayout.PropertyField(_capacityProperty);
                }
            }

            EditorGUILayout.Space();

            if (serializedObject.isEditingMultipleObjects == false)
            {
                var instance = (SoundFXManager)target;
                EditorGUILayout.LabelField("Total Count", $"{instance.CountAll}");
                EditorGUILayout.LabelField("Active Count", $"{instance.CountActive}");
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnPoolUpdated(SoundFXManager manager)
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                return;
            }

            if (manager != target)
            {
                return;
            }

            Repaint();
        }
    }
}
