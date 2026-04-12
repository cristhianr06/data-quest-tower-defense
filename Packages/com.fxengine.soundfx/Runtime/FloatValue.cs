using System;
using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Represents a constant value or a curve for an AudioSource setting.
    /// </summary>
    [Serializable]
    public struct FloatValue
    {
        public enum Type
        {
            Constant,
            Curve,
        }

        [SerializeField]
        private Type _type;

        [SerializeField]
        private float _constantValue;

        [SerializeField]
        private AnimationCurve _curve;

        public Type ValueType
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// The value to use when <see cref="ValueType"/> is <see cref="Type.Constant"/>.
        /// </summary>
        public float ConstantValue
        {
            get => _constantValue;
            set => _constantValue = value;
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

        public FloatValue(float constant)
        {
            _type = Type.Constant;
            _constantValue = constant;
            _curve = default;
        }
    }

    /// <summary>
    /// An equivalent of <see cref="RangeAttribute"/> designed to be used on <see cref="FloatValue"/> fields.
    /// </summary>
    public class FloatValueRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;
        
        public FloatValueRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
