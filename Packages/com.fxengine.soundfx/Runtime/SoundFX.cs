using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// A fancy AudioSource that can play overlapping instances of a sound.
    /// </summary>
    [AddComponentMenu("SoundFX/Sound FX")]
    public class SoundFX : MonoBehaviour, ISoundFXPlayable
    {
        [SerializeField]
        private AudioResource _audioResource;

        /// <summary>
        /// The audio resource (ex audio clip) to play.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-resource.html">AudioSource.resource</a>.
        /// </summary>
        public AudioResource AudioResource
        {
            get => _audioResource;
            set => _audioResource = value;
        }

        [SerializeField]
        private AutoPlayMode _autoPlayMode;

        /// <summary>
        /// Controls the automatic playback of this sound.
        /// </summary>
        public AutoPlayMode AutoPlayMode
        {
            get => _autoPlayMode;
            set => _autoPlayMode = value;
        }

        [SerializeField]
        private PositionMode _positionMode;

        /// <summary>
        /// Controls how this sound is managed in 3D space.
        /// </summary>
        public PositionMode PositionMode
        {
            get => _positionMode;
            set => _positionMode = value;
        }

        [SerializeField]
        private SoundFlags _soundFlags;

        /// <summary>
        /// Each flag corresponds to a boolean setting on <a href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</a>.
        /// </summary>
        public SoundFlags SoundFlags
        {
            get => _soundFlags;
            set => _soundFlags = value;
        }

        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 1f;

        /// <summary>
        /// The volume of the playback.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-volume.html">AudioSource.volume</a>.
        /// </summary>
        public float Volume
        {
            get => _volume;
            set => _volume = value;
        }

        [Range(0.0001f, 3f)]
        [SerializeField]
        private float _pitch = 1f;

        /// <summary>
        /// The pitch of the playback (making it sound higher or lower).
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-pitch.html">AudioSource.pitch</a>.
        /// </summary>
        public float Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }

        [Range(0, 256)]
        [SerializeField]
        private int _priority = 128;

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-priority.html">AudioSource.priority</a>.
        /// </summary>
        public int Priority
        {
            get => _priority;
            set => _priority = value;
        }

        [SerializeField]
        private AudioMixerGroup _output;

        /// <summary>
        /// Related to the <a href="https://docs.unity3d.com/Manual/AudioMixer.html">Audio Mixer workflow</a>.
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-outputAudioMixerGroup.html">AudioSource.outputAudioMixerGroup</a>.
        /// </summary>
        /// <remarks>If null then the global <see cref="SoundFXSettings.DefaultOutput">SoundFXSettings.DefaultOutput</see> will be used.</remarks>
        public AudioMixerGroup Output
        {
            get => _output;
            set => _output = value;
        }

        [SerializeField]
        private SoundFX3DSettings _settings3D;

        /// <summary>
        /// The 3D settings of this sound.
        /// Ignored if <see cref="PositionMode"/> is <see cref="PositionMode.NonSpatial">PositionMode.NonSpatial</see>.
        /// </summary>
        /// <remarks>If null then the global <see cref="SoundFXSettings.Default3DSettings">SoundFXSettings.Default3DSettings</see> will be used.</remarks>
        public SoundFX3DSettings Settings3D
        {
            get => _settings3D;
            set => _settings3D = value;
        }

#pragma warning disable CS0414
        [HideInInspector]
        [SerializeField]
        private int _version = 0;
#pragma warning restore CS0414

        /// <summary>
        /// A callback that is invoked just before the sound is played.
        /// Subscribe to configure the AudioSource before playback.
        /// </summary>
        public event Action<SoundFX, AudioSource> OnBeforePlay;

        private SoundFXInstance _instance;

        private readonly List<SoundFXInstance> _instances = new List<SoundFXInstance>();

        internal ICollection<SoundFXInstance> Instances => _instances;

        /// <summary>
        /// The AudioSource used by the last played instance of the sound.
        /// </summary>
        /// <remarks>Not guaranteed to be the same instance between separate playbacks due to pooling.</remarks>
        public AudioSource AudioSource => _instance is { IsValid: true } ? _instance.AudioSource : null;

        protected virtual void Start()
        {
            if (_autoPlayMode == AutoPlayMode.Start)
            {
                Play();
            }
        }

        protected virtual void OnEnable()
        {
            if (_autoPlayMode == AutoPlayMode.OnEnable)
            {
                Play();
            }
        }

        /// <summary>
        /// Plays a new instance of the sound.
        /// Unlike a regular AudioSource, this doesn't interrupt active playbacks of the sound.
        /// </summary>
        public virtual void Play()
        {
            Play(out _);
        }

        /// <summary>
        /// Stops the most recent playback.
        /// </summary>
        /// <remarks>
        /// If other instances are playing, they can be stopped using their <see cref="SoundFXInstanceHandle"/> obtained from <see cref="Play(out SoundFXInstanceHandle)"/>.
        /// </remarks>
        public virtual void Stop()
        {
            _instance?.Stop();
        }

        /// <summary>
        /// Stops all active instances of this sound.
        /// </summary>
        public virtual void StopAll()
        {
            foreach (var instance in _instances)
            {
                instance.Stop();
            }
        }

        /// <summary>
        /// Plays a new instance of the sound.
        /// Unlike a regular AudioSource, this doesn't interrupt active playbacks of the sound.
        /// </summary>
        /// <param name="handle">A handle that controls this playback instance (stop, fade in/out, etc.)</param>
        public virtual void Play(out SoundFXInstanceHandle handle)
        {
            if (SoundFXManager.InstanceChecked.TryGet(out var audioSource) == false)
            {
                _instance = default;
                handle = default;
                return;
            }

            _instance = new SoundFXInstance()
            {
                AudioSource = audioSource,
            };
            _instances.Add(_instance);

            var soundFXLifetime = audioSource.GetComponent<SoundFXLifetime>();
            soundFXLifetime.Play(this, _instance);

            SetupAudioSource(audioSource);
            OnBeforePlay?.Invoke(this, audioSource);
            audioSource.Play();

            // Query the original volume after all other setup steps
            _instance.OriginalVolume = audioSource.volume;

            handle = new SoundFXInstanceHandle(_instance);
        }

        internal void Release(SoundFXInstance instance)
        {
            for (var i = 0; i < _instances.Count; i++)
            {
                var candidate = _instances[i];

                if (candidate == instance)
                {
                    _instances.RemoveAt(i);
                    break;
                }
            }
        }

        protected virtual void SetupAudioSource(AudioSource audioSource)
        {
            audioSource.resource                = _audioResource;
            audioSource.volume                  = _volume;
            audioSource.pitch                   = _pitch;
            audioSource.priority                = _priority;
            audioSource.outputAudioMixerGroup   = _output;

            audioSource.loop                    = _soundFlags.HasSoundFlag(SoundFlags.Loop);
            audioSource.bypassEffects           = _soundFlags.HasSoundFlag(SoundFlags.BypassEffects);
            audioSource.bypassListenerEffects   = _soundFlags.HasSoundFlag(SoundFlags.BypassListenerEffects);
            audioSource.bypassReverbZones       = _soundFlags.HasSoundFlag(SoundFlags.BypassReverbZones);
            audioSource.ignoreListenerPause     = _soundFlags.HasSoundFlag(SoundFlags.IgnoreListenerPause);
            audioSource.ignoreListenerVolume    = _soundFlags.HasSoundFlag(SoundFlags.IgnoreListenerVolume);
            audioSource.spatialize              = _soundFlags.HasSoundFlag(SoundFlags.Spatialize);
            audioSource.spatializePostEffects   = _soundFlags.HasSoundFlag(SoundFlags.SpatializePostEffects);

            if (TryResolve3DSettings(out var settings3D))
            {
                Apply3DSettings(audioSource, settings3D);
            }
            else
            {
                // Edge case when the default 3D settings don't exist
                // Use the Unity AudioSource defaults
                audioSource.minDistance = 1f;
                audioSource.maxDistance = 500f;
                audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                audioSource.spread = 0f;
                audioSource.reverbZoneMix = 1f;
                audioSource.dopplerLevel = 1f;
                audioSource.panStereo = 0f;
            }

            switch (_positionMode)
            {
                case PositionMode.SpatialStatic:
                    var t = transform;
                    audioSource.transform.SetPositionAndRotation(t.position, t.rotation);
                    break;

                case PositionMode.SpatialDynamic:
                    var spatializer = audioSource.GetComponent<SoundFXSpatializer>();
                    spatializer.SetFollowTarget(transform);
                    break;

                case PositionMode.NonSpatial:
                    audioSource.spatialBlend = 0f;
                    audioSource.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    break;
            }
        }

        private void Apply3DSettings(AudioSource audioSource, SoundFX3DSettings settings)
        {
            audioSource.minDistance     = settings.MinDistance;
            audioSource.maxDistance     = settings.MaxDistance;
            audioSource.dopplerLevel    = settings.DopplerLevel;
            audioSource.panStereo       = settings.StereoPan;

            switch (settings.VolumeRolloff.ValueType)
            {
                case RolloffValue.Type.Logarithmic:
                    audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                    break;
                case RolloffValue.Type.Linear:
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                    break;
                case RolloffValue.Type.Curve:
                    audioSource.rolloffMode = AudioRolloffMode.Custom;
                    audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.VolumeRolloff.Curve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.VolumeRolloff.ValueType), settings.VolumeRolloff.ValueType.ToString());
            }

            switch (settings.SpatialBlend.ValueType)
            {
                case FloatValue.Type.Constant:
                    audioSource.spatialBlend = settings.SpatialBlend.ConstantValue;
                    break;
                case FloatValue.Type.Curve:
                    audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.SpatialBlend.Curve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.SpatialBlend.ValueType), settings.SpatialBlend.ValueType.ToString());
            }

            switch (settings.Spread.ValueType)
            {
                case FloatValue.Type.Constant:
                    audioSource.spread = settings.Spread.ConstantValue;
                    break;
                case FloatValue.Type.Curve:
                    audioSource.SetCustomCurve(AudioSourceCurveType.Spread, settings.Spread.Curve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.Spread.ValueType), settings.Spread.ValueType.ToString());
            }

            switch (settings.ReverbZoneMix.ValueType)
            {
                case FloatValue.Type.Constant:
                    audioSource.reverbZoneMix = settings.ReverbZoneMix.ConstantValue;
                    break;
                case FloatValue.Type.Curve:
                    audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.ReverbZoneMix.Curve);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.ReverbZoneMix.ValueType), settings.ReverbZoneMix.ValueType.ToString());
            }
        }

        /// <summary>
        /// Returns the effective 3D settings used by this sound.
        /// </summary>
        public bool TryResolve3DSettings(out SoundFX3DSettings settings)
        {
            if (_settings3D != null)
            {
                settings = _settings3D;
                return true;
            }

            if (SoundFXSettings.Instance != null)
            {
                if (SoundFXSettings.Instance.Default3DSettings != null)
                {
                    settings = SoundFXSettings.Instance.Default3DSettings;
                    return true;
                }
            }

            settings = default;
            return false;
        }
    }
}
