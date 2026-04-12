// System
using System;

// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Represents a celestial body in the form of a planet within a virtual space simulation. 
    /// Extends the functionality of the base <see cref="CelestialBody"/> class to include specific 
    /// attributes and behavior unique to planets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Planet"/> class is a specialized implementation of a <see cref="CelestialBody"/> 
    /// tailored to represent planets in a virtual space environment.
    /// </para>
    /// <para>
    /// This class extends the base functionality of <see cref="CelestialBody"/> by implementing 
    /// planet-specific logic for awakening, respawning, and handling the despawn event. It also 
    /// integrates with the <see cref="ScenePool"/> to manage the pooling and respawning of planet 
    /// instances, ensuring efficient resource utilization.
    /// </para>
    /// <para>
    /// Planets created from this class are registered with the <see cref="ScenePool"/> as blueprints 
    /// upon awakening if not already registered. The <see cref="Respawn"/> method generates a random 
    /// position within the specified spawn radius, allowing planets to be respawned within the 
    /// simulated space. The <see cref="OnDespawn"/> method, triggers the respawn process when the planet 
    /// is despawned.
    /// </para>
    /// </remarks>
    public class Planet : CelestialBody
    {
        /// <summary>
        /// Gets or sets the name of the planet.
        /// </summary>
        public string PlanetName = "";

        /// <summary>
        /// Gets or sets the radius within which the planet can be spawned.
        /// </summary>
        public float SpawnRadius = 18f;

        /// <summary>
        /// Gets or sets the minimum radius for spawning the planet.
        /// </summary>
        public float SpawnMinRadius = 16f;

        /// <summary>
        /// Called when the planet is awakened.
        /// </summary>
        /// <remarks>
        /// Overrides the base <see cref="CelestialBody"/> method to register the planet as a blueprint 
        /// with the <see cref="ScenePool"/> if it is not already registered.
        /// </remarks>
        protected override void Awake()
        {
            // Call base method.
            base.Awake();

            // Register the planet with the ScenePool, when not already registered.
            if (!ScenePool.Instance.HasBlueprint(this.PlanetName))
            {
                // Register this planet as a blueprint.
                ScenePool.Instance.Register(this.PlanetName, this.gameObject);
            }
        }

        /// <summary>
        /// Respawns the planet at a random position within the specified spawn radius.
        /// </summary>
        /// <remarks>
        /// The planet is respawned using the <see cref="ScenePool"/> to efficiently manage object pooling 
        /// and respawning.
        /// </remarks>
        private void Respawn()
        {
            // Create a random position within the spawn radius.
            Vector3 position = new Vector3(UnityEngine.Random.Range(SpawnMinRadius, SpawnRadius), 0, UnityEngine.Random.Range(SpawnMinRadius, SpawnRadius));

            // Randomize the sign of the position.
            position.x *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            position.z *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

            // Spawn the same planet using the ScenePool.
            ScenePool.Instance.Spawn(this.PlanetName, position, Quaternion.identity);
        }

        /// <summary>
        /// Called when the planet is despawned.
        /// </summary>
        /// <remarks>
        /// Overrides the base <see cref="CelestialBody"/> method to trigger the respawn process when the planet 
        /// is despawned, ensuring continuous dynamic behavior during gameplay.
        /// </remarks>
        public override void OnDespawn()
        {
            // Call the base method.
            base.OnDespawn();

            // Respawn the planet, if the game is still running.
            if (Application.isPlaying)
            {
                this.Respawn();
            }
        }
    }
}