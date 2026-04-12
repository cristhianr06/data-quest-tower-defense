using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Plays a <see cref="FXEngine.SoundFX.SoundFX"/> but ensures there's only ever one instance.
    /// Is also able to fade volume in/out.
    /// Starting a fade on top of another fade is supported (the older fade is cancelled and the new fade starts from the current volume).
    /// </summary>
    [AddComponentMenu("SoundFX/Sound FX Track")]
    [RequireComponent(typeof(SoundFX))]
    public class SoundFXTrack : MonoBehaviour, ISoundFXPlayable
    {
        [SerializeField]
        private float _fadeInDuration;

        /// <summary>
        /// The duration in seconds of the volume fade-in when <see cref="Play()"/> is called.
        /// Can be 0 to skip fade-in entirely.
        /// </summary>
        public float FadeInDuration
        {
            get => _fadeInDuration;
            set => _fadeInDuration = value;
        }

        [SerializeField]
        private float _fadeOutDuration = 0.1f;

        /// <summary>
        /// The duration in seconds of the volume fade-out when <see cref="Stop()"/> is called.
        /// Can be 0 to skip fade-out entirely.
        /// </summary>
        public float FadeOutDuration
        {
            get => _fadeOutDuration;
            set => _fadeOutDuration = value;
        }

        /// <summary>
        /// The SoundFX managed by this component.
        /// This is a sibling component on the same GameObject.
        /// </summary>
        public SoundFX SoundFX
        {
            get
            {
                // Handle the case where Awake() has not been called (ex after domain reload)
                if (_soundFX == null)
                {
                    _soundFX = GetComponent<SoundFX>();
                }

                return _soundFX;
            }
        }

        /// <summary>
        /// The current AudioSource instance.
        /// </summary>
        /// <remarks>Not guaranteed to be the same instance between separate playbacks due to pooling.</remarks>
        public AudioSource AudioSource => SoundFX.AudioSource;

        private SoundFX _soundFX;
        private SoundFXInstanceHandle _handle;

        protected void OnValidate()
        {
            _fadeInDuration = Mathf.Max(_fadeInDuration, 0f);
            _fadeOutDuration = Mathf.Max(_fadeOutDuration, 0f);
        }

        protected virtual void Awake()
        {
            _soundFX = GetComponent<SoundFX>();
        }

        /// <summary>
        /// Plays or fades in the sound.
        /// The sound is not re-started if it was already playing.
        /// </summary>
        /// <seealso cref="FadeInDuration"/>
        public virtual void Play()
        {
            Play(out _);
        }

        /// <summary>
        /// Plays or fades in the sound.
        /// The sound is not re-started if it was already playing.
        /// </summary>
        public virtual void Play(out SoundFXInstanceHandle handle)
        {
            // If no instance is currently playing, start a new instance
            if (_handle.IsValid == false)
            {
                SoundFX.Play(out _handle);
            }

            // Fade in the instance.
            // If it's a new instance, it will fade from 0.
            // If not, it will fade from the current volume.
            _handle.FadeIn(_fadeInDuration);

            handle = _handle;
        }

        /// <summary>
        /// Stops or fades out the sound.
        /// </summary>
        /// <seealso cref="FadeOutDuration"/>
        public virtual void Stop()
        {
            if (_handle.IsValid)
            {
                _handle.FadeOut(_fadeOutDuration);
            }
        }
    }
}
