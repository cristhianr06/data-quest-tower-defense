using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// An asset containing 3D settings that can be re-used across <see cref="FXEngine.SoundFX.SoundFX"/> instances.
    /// </summary>
    [CreateAssetMenu(fileName = "SoundFX 3D Settings", menuName = "SoundFX/3D Settings")]
    public class SoundFX3DSettings : ScriptableObject
    {
        [SerializeField]
        private float _minDistance = 1f;

        /// <summary>
        /// The volume stops increasing at this distance.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-minDistance.html">AudioSource.minDistance</a>.
        /// </summary>
        public float MinDistance
        {
            get => _minDistance;
            set => _minDistance = Mathf.Max(value, 0f);
        }

        [SerializeField]
        private float _maxDistance = 500f;

        /// <summary>
        /// The volume stops decreasing at this distance.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-maxDistance.html">AudioSource.maxDistance</a>.
        /// </summary>
        public float MaxDistance
        {
            get => _maxDistance;
            set => _maxDistance = Mathf.Max(value, 0f);
        }

        [Header("Levels")]

        [SerializeField]
        private RolloffValue _volumeRolloff = new RolloffValue(RolloffValue.Type.Logarithmic);

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-rolloffMode.html">AudioSource.rolloffMode</a> and <a href="https://docs.unity3d.com/ScriptReference/AudioSourceCurveType.CustomRolloff.html">AudioSourceCurveType.CustomRolloff</a>.
        /// </summary>
        public RolloffValue VolumeRolloff
        {
            get => _volumeRolloff;
            set => _volumeRolloff = value;
        }

        [Space]
        [FloatValueRange(0f, 1f)]
        [SerializeField]
        private FloatValue _spatialBlend = new FloatValue(1f);

        /// <summary>
        /// <para>When 0 the sound is treated as a 2D sound (not spatialized).</para>
        /// <para>When 1 the sound is treated as a 3D sound (fully spatialized).</para>
        /// <para>It can be useful to use a custom curve for this property to make the sound 2D when nearby, and transition towards 3D as the listener gets further.</para>
        /// <para>Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-spatialBlend.html">AudioSource.spatialBlend</a> and <a href="https://docs.unity3d.com/ScriptReference/AudioSourceCurveType.SpatialBlend.html">AudioSourceCurveType.SpatialBlend</a>.</para>
        /// </summary>
        public FloatValue SpatialBlend
        {
            get => _spatialBlend;
            set => _spatialBlend = value;
        }

        [Space]
        [FloatValueRange(0f, 360f)]
        [SerializeField]
        private FloatValue _spread = new FloatValue(0f);

        /// <summary>
        /// <para>The angle of the sound emission (for stereo spatialization purposes).</para>
        /// <para>Set this to 0 to have full panning (ex for a gunshot).</para>
        /// <para>Set this to 180 to have no panning at all (ex for a waterfall), note that distance attenuation still applies in this case.</para>
        /// <para>Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-spread.html">AudioSource.spread</a> and <a href="https://docs.unity3d.com/ScriptReference/AudioSourceCurveType.Spread.html">AudioSourceCurveType.Spread</a>.</para>
        /// </summary>
        public FloatValue Spread
        {
            get => _spread;
            set => _spread = value;
        }

        [Space]
        [FloatValueRange(0f, 1.1f)]
        [SerializeField]
        private FloatValue _reverbZoneMix = new FloatValue(1f);

        /// <summary>
        /// How strongly <a href="https://docs.unity3d.com/Documentation/Manual/class-AudioReverbZone.html">reverb zones</a> are applied to this sound.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-reverbZoneMix.html">AudioSource.reverbZoneMix</a> and <a href="https://docs.unity3d.com/ScriptReference/AudioSourceCurveType.ReverbZoneMix.html">AudioSourceCurveType.ReverbZoneMix</a>.
        /// </summary>
        public FloatValue ReverbZoneMix
        {
            get => _reverbZoneMix;
            set => _reverbZoneMix = value;
        }

        [Header("Misc")]

        [Range(0f, 5f)]
        [SerializeField]
        private float _dopplerLevel = 1f;

        /// <summary>
        /// Strength of the <a href="https://en.wikipedia.org/wiki/Doppler_effect">doppler effect</a>.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-dopplerLevel.html">AudioSource.dopplerLevel</a>.
        /// </summary>
        public float DopplerLevel
        {
            get => _dopplerLevel;
            set => _dopplerLevel = value;
        }

        [Range(-1f, 1f)]
        [SerializeField]
        private float _stereoPan = 0f;

        /// <summary>
        /// Pans the sound left (-1.0) or right (1.0). This pan is applied before 3D spatialization is computed.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-panStereo.html">AudioSource.panStereo</a>.
        /// </summary>
        public float StereoPan
        {
            get => _stereoPan;
            set => _stereoPan = value;
        }

        protected virtual void Reset()
        {
            _volumeRolloff.Curve = AnimationCurve.EaseInOut(0f, 1f, 500f, 0f);
            _spatialBlend.Curve = AnimationCurve.Constant(0f, 1f, 0f);
            _spread.Curve = AnimationCurve.Constant(0f, 1f, 0f);
            _reverbZoneMix.Curve = AnimationCurve.Constant(0f, 1f, 1f);
        }

        protected virtual void OnValidate()
        {
            if (_volumeRolloff.ValueType != RolloffValue.Type.Curve)
            {
                if (_maxDistance < _minDistance)
                {
                    _maxDistance = _minDistance + 0.000001f;
                }
            }

            _minDistance = Mathf.Max(_minDistance, 0f);
            _maxDistance = Mathf.Max(_maxDistance, 0f);
        }
    }
}
