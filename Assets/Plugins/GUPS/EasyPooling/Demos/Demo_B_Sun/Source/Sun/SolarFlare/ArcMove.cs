// Unity
using UnityEngine;

// GUPS
using GUPS.EasyPooling.Pooling;

namespace GUPS.EasyPooling.Demo.B
{
    /// <summary>
    /// Represents a poolable GameObject exhibiting arc movement with gravity towards the origin (0, 0).
    /// Multiple implementation of <see cref="IPoolAble"/> can be attached with a single GameObject.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ArcMove"/> class extends <see cref="MonoBehaviour"/> and implements the <see cref="IPoolAble"/> interface,
    /// making it suitable for use in object pooling scenarios. Instances of this class exhibit arc movement with a simulated
    /// gravitational pull towards the origin (0, 0).
    /// </para>
    /// <para>
    /// The <see cref="OnCreate"/> method is invoked upon the instantiation of a new object by the pool, finding and
    /// initializing the <see cref="Rigidbody"/> component reference. The <see cref="OnSpawn"/> method configures the
    /// object's initial velocity, introducing randomization to create unique movement patterns.
    /// </para>
    /// <para>
    /// The <see cref="Update"/> method adjusts the object's velocity over time to simulate gravitational pull towards the
    /// origin. It calculates a gravity direction vector based on the object's position and updates the velocity accordingly.
    /// </para>
    /// <para>
    /// The <see cref="OnDespawn"/> method is called when the object is despawned by the pool, and <see cref="OnDestroy"/>
    /// is called upon the object's destruction. Both methods provide hooks for additional cleanup or customization if needed,
    /// but are not required for this demo object to function properly.
    /// </para>
    /// </remarks>
    public class ArcMove : MonoBehaviour, IPoolAble
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
        /// The Rigidbody component reference.
        /// </summary>
        private Rigidbody rigidbody3D;

        /// <summary>
        /// Called when the object is newly created (instantiated) by the pool.
        /// </summary>
        public void OnCreate()
        {
            // Find the Rigidbody component reference.
            this.rigidbody3D = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called when the object is spawned by the pool, configuring initial velocity and introducing randomness.
        /// </summary>
        public void OnSpawn()
        {
            // Assign a random y-velocity to the object, allowing upward movement.
            this.rigidbody3D.linearVelocity += new Vector3(0, Random.Range(0f, 2f), 0);

            // Scale the velocity, assigned by the spawn policy, with a random factor for variation.
            this.rigidbody3D.linearVelocity *= Random.Range(0.5f, 1.5f);
        }

        /// <summary>
        /// Updates the object's velocity over time to simulate gravity towards the origin (0, 0).
        /// </summary>
        protected virtual void Update()
        {
            // Update velocity over time to simulate gravity towards (0, 0).
            Vector3 gravityDirection = -transform.position.normalized;
            this.rigidbody3D.linearVelocity += gravityDirection * Time.deltaTime;
        }

        /// <summary>
        /// Called when the object is despawned by the pool, always executed before <see cref="OnDestroy"/>.
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