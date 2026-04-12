using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    [CustomEditor(typeof(SoundFXSettings))]
    class SoundFXSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _defaultOutputProperty;
        private SerializedProperty _default3DSettingsProperty;

        private void OnEnable()
        {
            _defaultOutputProperty = serializedObject.FindProperty("DefaultOutput");
            _default3DSettingsProperty = serializedObject.FindProperty("Default3DSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(_defaultOutputProperty);
            EditorGUILayout.PropertyField(_default3DSettingsProperty);

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset"))
            {
                Undo.RecordObject(target, $"Reset {nameof(SoundFXSettings)}");

                var instance = (SoundFXSettings)target;
                instance.Reset();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
