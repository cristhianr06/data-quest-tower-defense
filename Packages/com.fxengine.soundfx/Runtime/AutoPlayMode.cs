namespace FXEngine.SoundFX
{
    /// <summary>
    /// Controls the automatic playback of a sound.
    /// </summary>
    public enum AutoPlayMode
    {
        /// <summary>
        /// Automatic playback is disabled.
        /// </summary>
        None,

        /// <summary>
        /// Playback begins on Start().
        /// </summary>
        Start,

        /// <summary>
        /// Playback begins on OnEnable().
        /// Contrary to <see cref="Start"/>, this can trigger multiple times over the component's lifetime.
        /// </summary>
        OnEnable,
    }
}
