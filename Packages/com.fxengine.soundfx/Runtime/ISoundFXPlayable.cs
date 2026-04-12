using UnityEngine;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Provides a common API for <see cref="SoundFX"/> and <see cref="SoundFXTrack"/>.
    /// </summary>
    public interface ISoundFXPlayable
    {
        /// <summary>
        /// Returns the AudioSource from the most recent playback.
        /// </summary>
        /// <remarks>
        /// Not guaranteed to return the same instance between playbacks (as a natural consequence of pooling).
        /// </remarks>
        AudioSource AudioSource { get; }

        /// <summary>
        /// Plays a new instance of the sound.
        /// </summary>
        void Play();

        /// <summary>
        /// Plays a new instance of the sound.
        /// </summary>
        /// <param name="handle">A handle that controls this playback instance (stop, fade in/out, etc.)</param>
        void Play(out SoundFXInstanceHandle handle);

        /// <summary>
        /// Stops the most recent playback.
        /// </summary>
        /// <remarks>
        /// If other instances are playing, they can be stopped using their <see cref="SoundFXInstanceHandle"/> obtained from <see cref="Play(out SoundFXInstanceHandle)"/>.
        /// </remarks>
        void Stop();
    }
}
