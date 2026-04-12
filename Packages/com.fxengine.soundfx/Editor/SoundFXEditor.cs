using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundFX))]
    class SoundFXEditor : UnityEditor.Editor
    {
        private static GUIContent _playButtonContent;

        private SerializedProperty _audioResourceProperty;
        private SerializedProperty _autoPlayModeProperty;
        private SerializedProperty _positionModeProperty;
        private SerializedProperty _soundFlagsProperty;
        private SerializedProperty _volumeProperty;
        private SerializedProperty _pitchProperty;
        private SerializedProperty _priorityProperty;
        private SerializedProperty _outputProperty;
        private SerializedProperty _settings3DProperty;

        private UnityEditor.Editor _settings3DEditor;

        private const string Settings3DFoldoutKey = "SoundFX/SoundFXEditor/Settings3DFoldout";
        private static bool Settings3DFoldout
        {
            get => EditorPrefs.GetBool(Settings3DFoldoutKey);
            set => EditorPrefs.SetBool(Settings3DFoldoutKey, value);
        }

        private const string DebugFoldoutKey = "SoundFX/SoundFXEditor/DebugFoldout";
        private static bool DebugFoldout
        {
            get => EditorPrefs.GetBool(DebugFoldoutKey);
            set => EditorPrefs.SetBool(DebugFoldoutKey, value);
        }

        private void OnEnable()
        {
            _audioResourceProperty = serializedObject.FindProperty("_audioResource");
            _autoPlayModeProperty = serializedObject.FindProperty("_autoPlayMode");
            _positionModeProperty = serializedObject.FindProperty("_positionMode");
            _soundFlagsProperty = serializedObject.FindProperty("_soundFlags");
            _volumeProperty = serializedObject.FindProperty("_volume");
            _pitchProperty = serializedObject.FindProperty("_pitch");
            _priorityProperty = serializedObject.FindProperty("_priority");
            _outputProperty = serializedObject.FindProperty("_output");
            _settings3DProperty = serializedObject.FindProperty("_settings3D");

            SoundFXManager.OnPoolUpdated += OnPoolUpdated;
        }

        private void OnDisable()
        {
            SoundFXManager.OnPoolUpdated -= OnPoolUpdated;

            if (_settings3DEditor != null)
            {
                DestroyImmediate(_settings3DEditor);
                _settings3DEditor = null;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            _playButtonContent ??= EditorGUIUtility.IconContent("d_PlayButton", "Play");

            var showControls = true;

            if (serializedObject.isEditingMultipleObjects == false)
            {
                // If a SoundFXTrack exists, it already shows the controls
                var soundFX = (SoundFX)target;
                showControls = soundFX.TryGetComponent(out SoundFXTrack _) == false;
            }

            if (showControls)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(_playButtonContent))
                    {
                        foreach (var t in targets)
                        {
                            var soundFX = (SoundFX)t;
                            soundFX.Play();
                        }
                    }

                    if (GUILayout.Button("■"))
                    {
                        foreach (var t in targets)
                        {
                            // Call StopAll() instead of Stop().
                            // Because this button is intended to be used for previewing mostly in the editor,
                            // and the user might have no other way to stop older instances (especially if the clip is long).
                            var soundFX = (SoundFX)t;
                            soundFX.StopAll();
                        }
                    }
                }

                EditorGUILayout.Space(1f);
            }

            EditorGUILayout.PropertyField(_audioResourceProperty);
            EditorGUILayout.PropertyField(_autoPlayModeProperty, new GUIContent("Auto Play"));
            EditorGUILayout.PropertyField(_positionModeProperty, new GUIContent("Position"));
            EditorGUILayout.PropertyField(_soundFlagsProperty, new GUIContent("Flags"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_volumeProperty);
            EditorGUILayout.PropertyField(_pitchProperty);
            EditorGUILayout.PropertyField(_priorityProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_outputProperty);

            if (_settings3DProperty.objectReferenceValue == null || _settings3DProperty.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(_settings3DProperty);
            }
            else
            {
                var rect = EditorGUILayout.GetControlRect();

                EditorGUI.BeginProperty(rect, new GUIContent(_settings3DProperty.displayName), _settings3DProperty);

                var foldoutRect = rect;
                foldoutRect.width = EditorGUIUtility.labelWidth;

                var labelRect = rect;
                labelRect.width = EditorGUIUtility.labelWidth;

                var fieldRect = rect;
                fieldRect.xMin = labelRect.xMax;

                Settings3DFoldout = EditorGUI.Foldout
                (
                    foldoutRect,
                    Settings3DFoldout,
                    GUIContent.none,
                    true
                );

                // Using PrefixLabel with GetControlID associates it with BeginProperty and PropertyField
                EditorGUI.PrefixLabel
                (
                    labelRect,
                    GUIUtility.GetControlID(FocusType.Passive),
                    new GUIContent(_settings3DProperty.displayName)
                );

                EditorGUI.PropertyField(fieldRect, _settings3DProperty, GUIContent.none);

                EditorGUI.EndProperty();

                // Show the 3D settings inspector
                // (disabled and shown as a preview to signal to the user that it's ScriptableObject data)
                if (Settings3DFoldout)
                {
                    using var indent = new EditorGUI.IndentLevelScope();
                    using var disabled = new EditorGUI.DisabledScope(true);

                    CreateCachedEditor(_settings3DProperty.objectReferenceValue, typeof(SoundFX3DSettingsEditor), ref _settings3DEditor);
                    if (_settings3DEditor != null)
                    {
                        _settings3DEditor.OnInspectorGUI();
                    }
                }
            }

            if (serializedObject.isEditingMultipleObjects == false)
            {
                EditorGUILayout.Space();

                DebugFoldout = EditorGUILayout.Foldout(DebugFoldout, new GUIContent("Debug"), true);
                if (DebugFoldout)
                {
                    using var indent = new EditorGUI.IndentLevelScope();
                    using var disabled = new EditorGUI.DisabledScope(true);

                    var soundFX = (SoundFX)target;

                    if (soundFX.Instances.Count == 0)
                    {
                        EditorGUILayout.LabelField("No active playback instances");
                    }

                    foreach (var instance in soundFX.Instances)
                    {
                        if (instance.IsValid)
                        {
                            EditorGUILayout.ObjectField(new GUIContent("Audio Source"), instance.AudioSource, typeof(AudioSource), true);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnPoolUpdated(SoundFXManager manager)
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                return;
            }

            Repaint();
        }

        private void OnSceneGUI()
        {
            if (target == null)
            {
                return;
            }

            var soundFX = (SoundFX)target;

            if (soundFX.PositionMode == PositionMode.NonSpatial)
            {
                return;
            }

            var previousColor = Handles.color;
            Handles.color = soundFX.enabled ?
                new Color(0.50f, 0.70f, 1.00f, 0.5f) :
                new Color(0.30f, 0.40f, 0.60f, 0.5f);

            var position = soundFX.transform.position;

            soundFX.TryResolve3DSettings(out var settings3D);
            RadiusHandle.DrawRadiusHandle(Quaternion.identity, position, settings3D.MinDistance);
            RadiusHandle.DrawRadiusHandle(Quaternion.identity, position, settings3D.MaxDistance);

            Handles.color = previousColor;
        }
    }
}
