using UnityEditor;

namespace FXEngine.SoundFX.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundFX3DSettings))]
    class SoundFX3DSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _minDistanceProperty;
        private SerializedProperty _maxDistanceProperty;
        private SerializedProperty _volumeRolloffProperty;
        private SerializedProperty _spatialBlendProperty;
        private SerializedProperty _spreadProperty;
        private SerializedProperty _reverbZoneMixProperty;
        private SerializedProperty _dopplerLevelProperty;
        private SerializedProperty _stereoPanProperty;

        private void OnEnable()
        {
            _minDistanceProperty = serializedObject.FindProperty("_minDistance");
            _maxDistanceProperty = serializedObject.FindProperty("_maxDistance");
            _volumeRolloffProperty = serializedObject.FindProperty("_volumeRolloff");
            _spatialBlendProperty = serializedObject.FindProperty("_spatialBlend");
            _spreadProperty = serializedObject.FindProperty("_spread");
            _reverbZoneMixProperty = serializedObject.FindProperty("_reverbZoneMix");
            _dopplerLevelProperty = serializedObject.FindProperty("_dopplerLevel");
            _stereoPanProperty = serializedObject.FindProperty("_stereoPan");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            if (_volumeRolloffProperty.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(_minDistanceProperty);
            }
            else
            {
                var volumeRolloffTypeProperty = _volumeRolloffProperty.FindPropertyRelative("_type");
                var volumeRolloffType = (RolloffValue.Type)volumeRolloffTypeProperty.enumValueIndex;

                if (volumeRolloffType != RolloffValue.Type.Curve)
                {
                    EditorGUILayout.PropertyField(_minDistanceProperty);
                }
            }

            EditorGUILayout.PropertyField(_maxDistanceProperty);
            EditorGUILayout.PropertyField(_volumeRolloffProperty);
            EditorGUILayout.PropertyField(_spatialBlendProperty);
            EditorGUILayout.PropertyField(_spreadProperty);
            EditorGUILayout.PropertyField(_reverbZoneMixProperty);
            EditorGUILayout.PropertyField(_dopplerLevelProperty);
            EditorGUILayout.PropertyField(_stereoPanProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
