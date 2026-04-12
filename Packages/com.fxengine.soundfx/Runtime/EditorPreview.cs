using System.Collections.Generic;
using UnityEngine;

namespace FXEngine.SoundFX
{
    class EditorPreview
    {
#if UNITY_EDITOR
        private static readonly List<IEditorPreviewComponent> _editorInstances = new List<IEditorPreviewComponent>();

        public static void OnEnable(IEditorPreviewComponent component)
        {
            _editorInstances.Add(component);
        }

        public static void OnDisable(IEditorPreviewComponent component)
        {
            _editorInstances.Remove(component);
        }

        public static void EditorInitialize()
        {
            // This is called at a much more regular rate than Update() on [ExecuteAlways] components,
            // because the latter relies on PlayerLoop updates that might not run at all.
            UnityEditor.EditorApplication.update += EditorUpdateInstances;
        }

        private static void EditorUpdateInstances()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }

            for (var i = 0; i < _editorInstances.Count; i++)
            {
                var instance = _editorInstances[i];

                if (instance == null)
                {
                    return;
                }

                instance.EditorUpdate(EditorTime.DeltaTime);
            }
        }
#else
        public static void OnEnable(IEditorPreviewComponent component)
        {
            
        }

        public static void OnDisable(IEditorPreviewComponent component)
        {
            
        }
#endif
    }

    interface IEditorPreviewComponent
    {
        void EditorUpdate(float deltaTime);
    }

#if UNITY_EDITOR
    // Time.deltaTime doesn't work in the editor. This is a substitute.
    static class EditorTime
    {
        private static double LastTimestamp = -1.0;

        private static float _deltaTime;

        public static float DeltaTime
        {
            get => Application.isPlaying ? Time.deltaTime : _deltaTime;
            private set => _deltaTime = value;
        }

        public static void EditorInitialize()
        {
            UnityEditor.EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            var newTimestamp = Time.realtimeSinceStartupAsDouble;

            if (LastTimestamp < 0)
                DeltaTime = 0f;
            else
                DeltaTime = (float)(newTimestamp - LastTimestamp);

            LastTimestamp = newTimestamp;
        }
    }

    static class EditorInitializer
    {
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            EditorTime.EditorInitialize();
            EditorPreview.EditorInitialize();
        }
    }
#endif
}
