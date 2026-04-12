// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Represents a component that automatically manages the lifespan of a GameObject by despawning it
    /// after a specified time duration has elapsed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TimeToLive"/> class provides a simple way to set a time limit for the existence 
    /// of a GameObject in a scene. Upon reaching the specified time duration, the GameObject is automatically 
    /// despawned back to the scene pool.
    /// </para>
    /// <para>
    /// The time to live is determined by the <see cref="TimeToLiveSeconds"/> property, which represents 
    /// the remaining seconds the GameObject has to live. The <see cref="Update"/> method continuously 
    /// updates the time to live, and once it reaches zero or below, the GameObject is despawned using 
    /// the <see cref="ScenePool"/> to efficiently manage object pooling and recycling.
    /// </para>
    /// </remarks>
    public class TimeToLive : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the time, in seconds, that the GameObject is allowed to exist before being despawned.
        /// </summary>
        public float TimeToLiveSeconds = 5f;

        /// <summary>
        /// Called every frame to update the time to live and handle automatic despawning.
        /// </summary>
        /// <remarks>
        /// The time to live is decremented based on the elapsed time since the last frame. If the time to live
        /// reaches zero or below, the GameObject is despawned back to the scene pool.
        /// </remarks>
        protected virtual void Update()
        {
            // Update the time to live.
            this.TimeToLiveSeconds -= Time.deltaTime;

            // Check if the time to live has expired.
            if (this.TimeToLiveSeconds <= 0f)
            {
                // Despawn the game object back to the scene pool.
                ScenePool.Instance.Despawn(this.gameObject);
            }
        }
    }
}
