// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Responsible for spawning asteroids within a specified radius and at a defined rate, utilizing object pooling for efficient management.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="AsteroidSpawner"/> class manages the spawning of asteroids within a designated radius. It leverages object pooling
    /// provided by the <see cref="ScenePool"/> instance for efficient instantiation and reuse of asteroids. The spawner can be configured
    /// with parameters such as the initial spawn count, spawn rate, and spawn radius.
    /// </para>
    /// <para>
    /// During initialization (<see cref="Start"/> method), the spawner spawns an initial batch of asteroids based on the configured
    /// <see cref="StartSpawnCount"/>. Subsequently, the <see cref="Spawn"/> method is invoked repeatedly at the specified <see cref="SpawnRate"/>
    /// to continually spawn asteroids within the specified radius.
    /// </para>
    /// </remarks>
    public class AsteroidSpawner : MonoBehaviour
    {
        /// <summary>
        /// The prefab representing the asteroid to be spawned.
        /// </summary>
        public GameObject Asteroid;

        /// <summary>
        /// The initial number of asteroids to spawn.
        /// </summary>
        public int StartSpawnCount = 50;

        /// <summary>
        /// The rate at which asteroids are spawned, in seconds.
        /// </summary>
        public float SpawnRate = 1f;

        /// <summary>
        /// The maximum distance from the center at which asteroids can be spawned.
        /// </summary>
        public float SpawnRadius = 18f;

        /// <summary>
        /// The minimum distance from the center at which asteroids can be spawned.
        /// </summary>
        public float SpawnMinRadius = 16f;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <remarks>
        /// The <see cref="Start"/> method is responsible for initializing the spawner. It spawns an initial batch of asteroids and sets up
        /// the repeating invocation of the <see cref="Spawn"/> method at the specified rate.
        /// </remarks>
        private void Start()
        {
            // Reduce the spawn min radius to 5 and temp store the min spawn radius.
            float tempSpawnMinRadius = this.SpawnMinRadius;
            this.SpawnMinRadius = 5f;

            // Spawn the initial asteroids.
            for (int i = 0; i < this.StartSpawnCount; i++)
            {
                this.Spawn();
            }

            // Restore the spawn min radius.
            this.SpawnMinRadius = tempSpawnMinRadius;

            // Invoke the Spawn method repeatedly.
            InvokeRepeating("Spawn", 0f, this.SpawnRate);
        }

        /// <summary>
        /// Spawns an asteroid at a random position within the specified radius.
        /// </summary>
        /// <remarks>
        /// The <see cref="Spawn"/> method generates a random position within the configured spawn radius, considering the specified
        /// minimum spawn radius. It then randomizes the position's sign and uses object pooling to spawn an asteroid at the calculated position.
        /// </remarks>
        private void Spawn()
        {
            // Create a random position within the spawn radius.
            Vector3 position = new Vector3(UnityEngine.Random.Range(SpawnMinRadius, SpawnRadius), 0, UnityEngine.Random.Range(SpawnMinRadius, SpawnRadius));

            // Randomize the sign of the position.
            position.x *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            position.z *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

            // Spawn the asteroid using the ScenePool.
            ScenePool.Instance.Spawn(this.Asteroid.name, position, Quaternion.identity);
        }
    }
}
