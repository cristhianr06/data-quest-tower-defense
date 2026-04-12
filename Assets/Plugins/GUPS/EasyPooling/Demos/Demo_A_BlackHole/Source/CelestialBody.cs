// System
using System;
using System.Collections;

// Unity
using UnityEngine;

// GUPS
using GUPS.EasyPooling.Pooling;
using GUPS.EasyPooling.Policy;
using GUPS.EasyPooling.Policy.Random;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Represents a celestial body in the scene that implements the <see cref="ICelestialBody"/> and <see cref="IPoolAble"/> interfaces.
    /// This class defines properties and methods for managing celestial bodies, including explosion behavior and pooling integration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="CelestialBody"/> class extends <see cref="MonoBehaviour"/> and implements the <see cref="ICelestialBody"/> and <see cref="IPoolAble"/> interfaces.
    /// It provides properties for controlling explosion distance, explosion particles, the owning pool, and whether the object is currently pooled.
    /// </para>
    /// <para>
    /// The explosion behavior is triggered in the <see cref="Update"/> method when the celestial body's distance from the blackhole (origin) is less than the specified
    /// explosion distance, and its position is within a predefined range around the X and Z axes. Upon explosion, the class spawns a defined number of explosion
    /// particles from the pool using a custom <see cref="SpawnPolicy"/> and then despawns the celestial body.
    /// </para>
    /// </remarks>
    public class CelestialBody : MonoBehaviour, ICelestialBody, IPoolAble
    {
        /// <summary>
        /// The distance from the origin at which the celestial body explodes.
        /// </summary>
        public float DistanceExplode = 1f;

        /// <summary>
        /// The explosion particle GameObject to be spawned upon explosion.
        /// </summary>
        public GameObject ExplosionParticle;

        /// <inheritdoc/>
        public IPool Owner { get; set; }

        /// <inheritdoc/>
        public bool IsPooled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the celestial body has exploded.
        /// </summary>
        private bool isExploded = false;

        /// <summary>
        /// The spawn policy used for spawning explosion particles.
        /// </summary>
        private SpawnPolicy explosionSpawnPolicy;

        /// <summary>
        /// Initializes the celestial body, creating the explosion spawn policy for explosion particles.
        /// </summary>
        protected virtual void Awake()
        {
            // Create the explosion spawn policy for explosion particles, which will be used when the celestial body explodes.
            this.explosionSpawnPolicy = new SpawnPolicyBuilder(new UniformRandom())
                // Spawn with center at Vector3.zero.
                .SetCenter(Vector3.zero)
                // Spawn around center in a 2D circle with radius 0.5, on the XZ plane, with positions above an below the X axis.
                .SpawnInCircle(0.5f, Shapes.EPlane.XZ, new Tuple<float, float>[] {
                new Tuple<float, float>(-0.05f * Mathf.PI - 0.5f * Mathf.PI, 0.05f * Mathf.PI - 0.5f * Mathf.PI),
                new Tuple<float, float>(-0.05f * Mathf.PI + 0.5f * Mathf.PI, 0.05f * Mathf.PI + 0.5f * Mathf.PI)
                })
                // Set the applied velocity to the rigidbody to 5.
                .SetVelocity(5f)
                // Build the spawn policy.
                .Build();
        }

        /// <summary>
        /// Called upon the creation of the celestial body.
        /// </summary>
        public virtual void OnCreate()
        {
            // Does nothing.
        }

        /// <summary>
        /// Called upon the spawning of the celestial body.
        /// </summary>
        public virtual void OnSpawn()
        {
            // Does nothing.
        }

        /// <summary>
        /// Called during each frame update, checks for conditions to trigger explosion, and performs explosion behavior.
        /// </summary>
        protected virtual void Update()
        {
            // If the celestial body has already exploded, do nothing.
            if(this.isExploded)
            {
                return;
            }

            // Else check if the celestial body should explode.
            if (Vector3.Distance(this.transform.position, Vector3.zero) < this.DistanceExplode)
            {
                if (Mathf.Abs(this.transform.position.x) < 1 || Mathf.Abs(this.transform.position.z) < 1)
                {
                    StartCoroutine(this.Explode());
                }
            }
        }

        /// <summary>
        /// Initiates the explosion behavior, spawning explosion particles and handling celestial body despawn.
        /// </summary>
        private IEnumerator Explode()
        {
            // The celestial body has exploded.
            this.isExploded = true;

            // Spawn 75 explosion particles with the explosion spawn policy.
            for (int i = 0; i < 5; i++)
            {
                // Spawn 15 explosion particle at once.
                ScenePool.Instance.SpawnMany(this.ExplosionParticle.name, 15, this.explosionSpawnPolicy);

                // Wait for the next update.
                yield return null;
            }

            // Despawn the celestial body and push it back to the pool or destroy it if the pool is full.
            ScenePool.Instance.Despawn(this.gameObject);
        }

        /// <summary>
        /// Called upon the despawning of the celestial body.
        /// </summary>
        public virtual void OnDespawn()
        {
            // Does nothing.
        }

        /// <summary>
        /// Called upon the destruction of the celestial body.
        /// </summary>
        public virtual void OnDestroy()
        {
            // Does nothing.
        }
    }
}