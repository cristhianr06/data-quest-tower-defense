using System.Linq;
using UnityEditor;

namespace FXEngine.SoundFX.Editor
{
    class SoundFXSettingsProvider : SettingsProvider
    {
        private UnityEditor.Editor _editor;

        private SoundFXSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
            if (SoundFXSettings.Instance != null)
            {
                var serializedObject = new SerializedObject(SoundFXSettings.Instance);
                keywords = GetSearchKeywordsFromSerializedObject(serializedObject).Concat(new[]{ "Audio", "Sound" });
                serializedObject.Dispose();
            }
        }

        public override void OnGUI(string searchContext)
        {
            if (SoundFXSettings.Instance != null)
            {
                UnityEditor.Editor.CreateCachedEditor(SoundFXSettings.Instance, typeof(SoundFXSettingsEditor), ref _editor);
                _editor.OnInspectorGUI();
            }
        }

        public override void OnDeactivate()
        {
            if (_editor != null)
            {
                UnityEngine.Object.DestroyImmediate(_editor);
                _editor = null;
            }
        } 

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new SoundFXSettingsProvider("Project/SoundFX", SettingsScope.Project);
        }
    }
}
