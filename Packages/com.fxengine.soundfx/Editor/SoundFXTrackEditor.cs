using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundFXTrack))]
    class SoundFXTrackEditor : UnityEditor.Editor
    {
        private static GUIContent _playButtonContent;

        private SerializedProperty _fadeInDurationProperty;
        private SerializedProperty _fadeOutDurationProperty;

        private void OnEnable()
        {
            _fadeInDurationProperty = serializedObject.FindProperty("_fadeInDuration");
            _fadeOutDurationProperty = serializedObject.FindProperty("_fadeOutDuration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            _playButtonContent ??= EditorGUIUtility.IconContent("d_PlayButton", "Play");

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(_playButtonContent))
                {
                    foreach (var t in targets)
                    {
                        var soundFXTrack = (SoundFXTrack)t;
                        soundFXTrack.Play();
                    }
                }

                if (GUILayout.Button("■"))
                {
                    foreach (var t in targets)
                    {
                        var soundFXTrack = (SoundFXTrack)t;
                        soundFXTrack.Stop();
                    }
                }
            }

            EditorGUILayout.Space(1f);

            EditorGUILayout.PropertyField(_fadeInDurationProperty);
            EditorGUILayout.PropertyField(_fadeOutDurationProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
