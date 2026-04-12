// Unity
using UnityEngine;

// System
using System.Collections;

// GUPS
using GUPS.EasyPooling.Policy;
using GUPS.EasyPooling.Policy.Random;

namespace GUPS.EasyPooling.Demo.B
{
    /// <summary>
    /// Spawns solar flares around the sun in the scene at specified intervals with customizable spawn policies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="SolarFlareSpawner"/> class provides functionality to spawn solar flares in the scene. Solar flares 
    /// are spawned from the pool at regular intervals, and the spawning behavior is defined by a spawn policy, which 
    /// includes parameters such as position, shape, and velocity.
    /// </para>
    /// <para>
    /// This spawner uses the <see cref="GlobalPool"/> for efficient object reuse, spawning instances of the specified
    /// <see cref="SolarFlare"/> prefab with the configured spawn policy.
    /// </para>
    /// <para>
    /// A spawn policy is built each time new solar flares should be spawned, creating a <see cref="SpawnPolicy"/> instance that
    /// specifies the center, position, and velocity for the solar flare particles.
    /// </para>
    /// </remarks>
    public class SolarFlareSpawner : MonoBehaviour
    {
        /// <summary>
        /// The prefab of the solar flare to be spawned.
        /// </summary>
        public GameObject SolarFlare;

        /// <summary>
        /// The time interval between consecutive spawns of solar flares.
        /// </summary>
        public float SpawnInterval = 1f;

        /// <summary>
        /// The time of the last spawn of a solar flare.
        /// </summary>
        private float lastSpawnTime = 0f;

        /// <summary>
        /// The total number of solar flares to be spawned.
        /// </summary>
        public float SpawnCount = 15f;

        /// <summary>
        /// Called on each frame update. Check if it is time to start a new coroutine to spawn solar flares.
        /// </summary>
        protected virtual void Update()
        {
            // Check if it is time to spawn a solar flare.
            if (Time.time - this.lastSpawnTime > this.SpawnInterval)
            {
                this.lastSpawnTime = Time.time;

                // Start the coroutine to spawn solar flares.
                StartCoroutine(this.SpawnSolarFlares());
            }
        }

        /// <summary>
        /// Spawns solar flares based on the configured parameters at regular intervals.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> used for coroutine functionality.</returns>
        private IEnumerator SpawnSolarFlares()
        {
            // Create a random direction for the solar flare to shoot out from the sun.
            float direction = UnityEngine.Random.Range(0f, 2f * Mathf.PI);

            // Create a random velocity for the solar flare to shoot out from the sun.
            float velocity = UnityEngine.Random.Range(2f, 8f);

            // Color the solar flares with a random color between ffb719 and e24a10.
            Color color = new Color(
                UnityEngine.Random.Range(225, 230) / 255f,
                UnityEngine.Random.Range(120, 150) / 255f,
                UnityEngine.Random.Range(15, 25) / 255f,
                1f
            );

            // Create a spawn policy with the random direction and velocity and apply it to the solar flare.
            var spawnPolicy = new SpawnPolicyBuilder(new UniformRandom())
                // Spawn with center at Vector3.zero.
                .SetCenter(Vector3.zero)
                // Spawn around center in a 2D circle with radius 0.5, on the XZ plane and with positions / velocity in the specified direction.
                .SpawnInCircle(0.5f, Shapes.EPlane.XZ, new System.Tuple<float, float>[]
                {
                    new System.Tuple<float, float>(direction - 0.1f, direction + 0.1f)
                })
                // Set the applied velocity to the rigidbody.
                .SetVelocity(velocity)
                // Build the spawn policy.
                .Build();

            // Spawn solar flares for the specified number of times.
            for (int i = 0; i < this.SpawnCount; i++)
            {
                // Spawn a solar flare using the GlobalPool and configured spawn policy.
                GameObject spawned = GlobalPool.Instance.Spawn(this.SolarFlare.name, spawnPolicy);

                // Assign them all the same color.
                spawned.GetComponent<SolarFlare>().Color = color;

                // Wait for next frame before spawning the next solar flare.
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}