using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Project settings for the SoundFX package.
    /// </summary>
    public class SoundFXSettings : ScriptableObject
    {
        private const string AssetPath = "Assets/Settings/Resources/SoundFX Settings.asset";
        private const string AssetFileName = "SoundFX Settings";

        private static SoundFXSettings _instance;

        public static SoundFXSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    Load();
                }

                return _instance;
            }
        }

        /// <summary>
        /// The mixer that is used when <see cref="SoundFX.Output">SoundFX.Output</see> is left blank.
        /// </summary>
        public AudioMixerGroup DefaultOutput;

        /// <summary>
        /// The 3D settings that are used when <see cref="SoundFX.Settings3D">SoundFX.Settings3D</see> is left blank and the sound is spatialized.
        /// </summary>
        public SoundFX3DSettings Default3DSettings;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeInEditor()
        {
            EditorApplication.delayCall += Load;
        }
#endif

        private static void Load()
        {
            _instance = Load(AssetPath, AssetFileName);
        }

        private static SoundFXSettings Load(string path, string filename)
        {
            var instance = Resources.Load<SoundFXSettings>(filename);

            if (instance == null)
            {
                instance = CreateInstance<SoundFXSettings>();

#if UNITY_EDITOR
                var parentDirectory = Path.GetDirectoryName(path);
                EnsureFoldersExist(parentDirectory);

                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
#endif
            }

            return instance;
        }

        // Called automatically when an instance is created
        public void Reset()
        {
            var default3DSettings = Resources.Load<SoundFX3DSettings>("SoundFX Default 3D Settings");
            Default3DSettings = default3DSettings;
        }

#if UNITY_EDITOR
        private static void EnsureFoldersExist(string path)
        {
            var parts = path.Split('/', '\\');
            var current = parts[0];

            for (var i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";

                if (AssetDatabase.IsValidFolder(next) == false)
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
#endif
    }
}
