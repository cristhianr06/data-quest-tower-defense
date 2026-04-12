using System;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Each flag corresponds to a boolean setting on <a href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</a>.
    /// </summary>
    [Flags]
    public enum SoundFlags
    {
        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-loop.html">AudioSource.loop</a>.
        /// </summary>
        Loop                    = 1 << 0,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-bypassEffects.html">AudioSource.bypassEffects</a>.
        /// </summary>
        BypassEffects           = 1 << 1,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-bypassListenerEffects.html">AudioSource.bypassListenerEffects</a>.
        /// </summary>
        BypassListenerEffects   = 1 << 2,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-bypassReverbZones.html">AudioSource.bypassReverbZones</a>.
        /// </summary>
        BypassReverbZones       = 1 << 3,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-ignoreListenerPause.html">AudioSource.ignoreListenerPause</a>.
        /// </summary>
        IgnoreListenerPause     = 1 << 4,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-ignoreListenerVolume.html">AudioSource.ignoreListenerVolume</a>.
        /// </summary>
        IgnoreListenerVolume    = 1 << 5,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-spatialize.html">AudioSource.spatialize</a>.
        /// </summary>
        /// <remarks>
        /// This is different from <see cref="SoundFX.PositionMode">SoundFX.PositionMode</see>.
        /// </remarks>
        Spatialize              = 1 << 6,

        /// <summary>
        /// Corresponds to <a href="https://docs.unity3d.com/ScriptReference/AudioSource-spatializePostEffects.html">AudioSource.spatializePostEffects</a>.
        /// </summary>
        /// <remarks>
        /// This is different from <see cref="SoundFX.PositionMode">SoundFX.PositionMode</see>.
        /// </remarks>
        SpatializePostEffects   = 1 << 7,
    }

    public static class SoundFlagsUtility
    {
        /// <summary>
        /// A version of <see cref="Enum.HasFlag">Enum.HasFlag</see> that avoids boxing.
        /// </summary>
        public static bool HasSoundFlag(this SoundFlags a, SoundFlags b)
        {
            return ((int)a & (int)b) == (int)b;
        }
    }
}
