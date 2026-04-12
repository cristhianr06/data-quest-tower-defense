using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Extends <see cref="IconAttribute"/> to avoid hard-coding the icon's path.
    /// </summary>
    class EmbeddedIconAttribute : IconAttribute
    {
#if UNITY_EDITOR
        public EmbeddedIconAttribute(string guid) : base(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)) { }
#else
        public EmbeddedIconAttribute(string guid) : base(null) { }
#endif
    }
}
