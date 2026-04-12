// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Represents an asteroid, a type of celestial body, inheriting properties and behaviors from the <see cref="CelestialBody"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Asteroid"/> class extends the functionality of a celestial body to specifically model an asteroid. As a subclass of
    /// <see cref="CelestialBody"/>, it inherits common properties and behaviors related to celestial bodies, such as pooling, spawning,
    /// and despawning.
    /// </para>
    /// <para>
    /// During the <see cref="OnSpawn"/> phase, the asteroid scales itself randomly.
    /// </para>
    /// </remarks>
    public class Asteroid : CelestialBody
    {
        /// <summary>
        /// Called upon the spawning of the asteroid, scales the asteroid randomly.
        /// </summary>
        public override void OnSpawn()
        {
            // Scale the asteroid randomly.
            this.transform.localScale = Vector3.one * Random.Range(0.01f, 0.02f);
        }
    }
}
