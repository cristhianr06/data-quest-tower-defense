using System;
using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Represents volume rolloff settings.
    /// </summary>
    [Serializable]
    public struct RolloffValue
    {
        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-rolloffMode.html">AudioSource.rolloffMode</a>.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A realistic rolloff. Ignores Max Distance.
            /// </summary>
            Logarithmic,

            /// <summary>
            /// Linearly interpolates the volume from 1 to 0 across Min Distance and Max Distance.
            /// </summary>
            Linear,

            /// <summary>
            /// A user-defined curve.
            /// The curve X and Y axis are expected in the 0-1 range, and will be automatically remapped to <see cref="SoundFX3DSettings.MaxDistance">SoundFX3DSettings.MaxDistance</see>.
            /// The last value of the curve is held beyond Max Distance.
            /// </summary>
            Curve,
        }

        [SerializeField]
        private Type _type;

        [SerializeField]
        private AnimationCurve _curve;

        public Type ValueType
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// The curve to use when <see cref="ValueType"/> is <see cref="Type.Curve"/>.
        /// </summary>
        /// <remarks>
        /// The X axis is expected to be in the 0-1 range, and will be automatically remapped to <see cref="SoundFX3DSettings.MaxDistance">SoundFX3DSettings.MaxDistance</see>.
        /// The last value of the curve is held beyond Max Distance.
        /// </remarks>
        public AnimationCurve Curve
        {
            get => _curve;
            set => _curve = value;
        }

        public RolloffValue(Type type)
        {
            _type = type;
            _curve = default;
        }
    }
}
