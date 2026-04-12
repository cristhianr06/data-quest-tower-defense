using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Manages a single playback instance of a <see cref="FXEngine.SoundFX.SoundFX"/>.
    /// Tied to an <a href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</a> that will be returned to a pool once playback is finished.
    /// </summary>
    public readonly struct SoundFXInstanceHandle
    {
        /// <summary>
        /// True if the playback is still active.
        /// </summary>
        public bool IsValid => _instance is { IsValid: true };

        /// <summary>
        /// Returns the AudioSource playing the sound.
        /// Returns null after the playback has completed.
        /// </summary>
        public AudioSource AudioSource => _instance.IsValid ? _instance.AudioSource : null;

        private readonly SoundFXInstance _instance;

        internal SoundFXInstanceHandle(SoundFXInstance instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// Stops the playback.
        /// </summary>
        public void Stop()
        {
            _instance.Stop();
        }

        /// <summary>
        /// Fades to the target volume, starting at the current volume.
        /// </summary>
        /// <param name="target">The target volume in the normalized 0-1 range. Will be remapped using the full volume that was determined when this handle was created.</param>
        /// <param name="duration">The fade duration in seconds.</param>
        /// <param name="durationMode">Specifies how to interpret the fade duration.</param>
        public void Fade(float target, float duration, FadeDurationMode durationMode = FadeDurationMode.ConstantSpeed)
        {
            _instance.Fade(target, duration, durationMode);
        }

        /// <summary>
        /// Fades in to full volume.
        /// If this handle was just created, the fade starts from 0, otherwise it starts from the current volume level.
        /// The full volume is determined when this handle was created.
        /// </summary>
        public void FadeIn(float duration)
        {
            _instance.FadeIn(duration);
        }

        /// <summary>
        /// Fades the volume out to zero, starting from the current volume level.
        /// </summary>
        public void FadeOut(float duration)
        {
            _instance.FadeOut(duration);
        }

        /// <summary>
        /// Sets the volume of the sound.
        /// Note: Can be overriden by any present or future volume fades.
        /// </summary>
        /// <remarks>Check <see cref="IsValid"/> before accessing.</remarks>
        public float Volume
        {
            get => AudioSource.volume;
            set => AudioSource.volume = value;
        }

        /// <summary>
        /// Sets the pitch of the sound.
        /// </summary>
        /// <remarks>Check <see cref="IsValid"/> before accessing.</remarks>
        public float Pitch
        {
            get => AudioSource.pitch;
            set => AudioSource.pitch = value;
        }
    }

    /// <summary>
    /// Specifies the duration and speed of a fade.
    /// </summary>
    public enum FadeDurationMode
    {
        /// <summary>
        /// <para>
        /// The fade will move at a constant speed regardless of the start and final values.
        /// It's assumed that the specified duration is for fading between 0 and 1, so the speed is derived from that and the total fade distance.
        /// For example a fade from values 0.5 to 1 with a duration of 0.2 seconds will take only 0.1 seconds because the fade distance is 0.5.
        /// </para>
        /// <para>
        /// This useful to allow an older fade to be interrupted by a newer fade.
        /// Overall volume consistency is reflected in the speed of the fade rather than the duration of individual fades.
        /// </para>
        /// </summary>
        ConstantSpeed,

        /// <summary>
        /// The fade will take the exact time specified by the user, regardless of the start and final values.
        /// </summary>
        ConstantTime,
    }
}
