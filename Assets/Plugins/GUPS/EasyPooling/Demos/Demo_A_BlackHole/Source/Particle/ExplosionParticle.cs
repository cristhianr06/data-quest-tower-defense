// Unity
using UnityEngine;

// GUPS
using GUPS.EasyPooling.Pooling;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Represents a particle for an explosion, designed to be efficient reuseable and managementable through object pooling.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ExplosionParticle"/> class extends <see cref="MonoBehaviour"/> and implements the <see cref="IPoolAble"/> interface
    /// to use object pooling. This class makes it possible to efficient spawn and despawn frequently the GameObjects representing the particles,
    /// by pooling them instead of destroying and creating them.
    /// </para>
    /// <para>
    /// The lifespan of an explosion particle is managed by the <see cref="TimeToLive"/> component attached to the same GameObject. The time to live
    /// is randomly initialized upon spawning, ensuring variability in the duration of each particle's existence. The <see cref="OnSpawn"/> method is
    /// called when the particle is created or reused from the pool, allowing for specific initialization procedures, such as setting a random time to live.
    /// </para>
    /// </remarks>
    public class ExplosionParticle : MonoBehaviour, IPoolAble
    {
        /// <summary>
        /// Gets or sets the <see cref="TimeToLive"/> component managing the lifespan of the explosion particle.
        /// </summary>
        public TimeToLive TimeToLive;

        /// <inheritdoc/>
        public IPool Owner { get; set; }

        /// <inheritdoc/>
        public bool IsPooled { get; set; }

        /// <summary>
        /// Called when the GameObject is first created, performs one-time setup.
        /// </summary>
        /// <remarks>
        /// This method is invoked once during the lifetime of the GameObject or the behavior. It is recommended
        /// to perform reusable reference initialization in this method.
        /// </remarks>
        protected virtual void Awake()
        {
            /* For poolable gameobjects, the lifecycle is a bit different, then you would normally use in unity.
             * Awake is always called on the behaviours, when a new gameobject (or the behaviour) is created.
             * But using pooling, you don't always create new gameobjects, when you need them, but instead reuse them.
             * So the Awake method is only called once, when the gameobject (or the behaviour) is new created.
             * 
             * So I would recommend to move your normal initialization code from Awake to OnSpawn, which is called every 
             * time the gameobject is new created or reused from the pool. Use Awake to initialize reusable references, 
             * like references to other Behaviours on the same gameobject.
             */
            this.TimeToLive = this.GetComponent<TimeToLive>();
        }

        /// <inheritdoc/>
        public void OnCreate()
        {
            // Do nothing.
        }

        /// <summary>
        /// Called when the GameObject is spawned.
        /// </summary>
        /// <remarks>
        /// This method is called every time the GameObject is spawned from the pool. It is recommended to perform game-specific 
        /// initialization, such as setting a random time to live in this demo, in this method.
        /// </remarks>
        public void OnSpawn()
        {
            // Apply a random time to live.
            this.TimeToLive.TimeToLiveSeconds = Random.Range(4f, 5f);
        }

        /// <inheritdoc/>
        public void OnDespawn()
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        public void OnDestroy()
        {
            // Do nothing.
        }
    }
}
