namespace FXEngine.SoundFX
{
    /// <summary>
    /// Controls how an AudioSource is managed in 3D space.
    /// </summary>
    public enum PositionMode
    {
        /// <summary>
        /// The AudioSource is "spawned" at the transform, but doesn't follow it over time.
        /// Use this for "instant" sounds like gunshots, explosions, impacts, etc.
        /// </summary>
        SpatialStatic,

        /// <summary>
        /// The AudioSource is attached to its transforms and follows it.
        /// Use this when the sound is "emitted" over a period of time from an object.
        /// Ex an NPC walking around and whistling.
        /// </summary>
        SpatialDynamic,

        /// <summary>
        /// The AudioSource is 2D and the distance/angle between the AudioSource and the listener isn't applied.
        /// Volume and stereo pan are expected to be controlled manually.
        /// </summary>
        NonSpatial,
    }
}
