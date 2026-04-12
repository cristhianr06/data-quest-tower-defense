// Unity
using UnityEngine;

// GUPS
using GUPS.EasyPooling.Pooling;

namespace GUPS.EasyPooling.Demo.B
{
    /// <summary>
    /// Represents a poolable solar flare GameObject with time-based lifespan, spawning out of the sun.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="SolarFlare"/> class implementing <see cref="IPoolAble"/>, defines a poolable GameObject that 
    /// simulates a sun solar flare. Each instance has a limited time to live, and after a randomly assigned duration 
    /// between 2 and 5 seconds, the object is automatically despawned.
    /// </para>
    /// <para>
    /// The <see cref="SolarFlare"/> class supports the <see cref="IPoolAble"/> interface, making it compatible with object
    /// pooling systems. It includes methods such as <see cref="OnCreate"/>, <see cref="OnSpawn"/>,
    /// <see cref="OnDespawn"/>, and <see cref="OnDestroy"/> to manage its lifecycle within the pool.
    /// </para>
    /// </remarks>
    public class SolarFlare : MonoBehaviour, IPoolAble
    {
        /// <summary>
        /// Gets or sets the pool that owns this instance.
        /// </summary>
        public IPool Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently pooled.
        /// </summary>
        public bool IsPooled { get; set; }

        /// <summary>
        /// Stores the reference to the mesh renderer component.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Get and set the color of the solar flare.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.meshRenderer.material.color;
            }
            set
            {
                this.meshRenderer.material.color = value;
            }
        }

        /// <summary>
        /// The time in seconds the object has to live at least, so it has a chance to move away from the sun.
        /// </summary>
        private float minTimeToLive = 2f;

        /// <summary>
        /// Called when the object is new created (instantiated) by the pool.
        /// </summary>
        public void OnCreate()
        {
            // Find the mesh renderer component.
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Called when the object is spawned.
        /// </summary>
        public void OnSpawn()
        {
            // Note: Resetting the minimum time to live is not required here, because using a blueprint to spawn an GameObject
            // will reset private/protected/public primitive data types, such as the minTimeToLive field in this demo.
            // this.minTimeToLive = 2f;

            // Give the object a random scale.
            float scale = Random.Range(0.05f, 0.2f);

            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        /// <summary>
        /// Called once per frame to update the object's behavior, decreasing the time to live and despawning if necessary.
        /// </summary>
        protected virtual void Update()
        {
            // Decrease the time to live.
            this.minTimeToLive -= Time.deltaTime;

            // If the time to live is bigger than zero, return. 
            if (this.minTimeToLive > 0.0f)
            {
                return;
            }

            // If the distance to the sun is smaller than 1.5, despawn the object.
            if (Vector3.Distance(this.transform.position, Vector3.zero) < 1.5f)
            {
                // You can also use the Owner property to despawn the object.
                this.Owner.Despawn(this.gameObject);
            }
        }

        /// <summary>
        /// Called when the object is despawned by the pool, always called before OnDestroy.
        /// </summary>
        public void OnDespawn()
        {
            // Do nothing on object despawn.
        }

        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        public void OnDestroy()
        {
            // Do nothing on object destruction.
        }
    }
}