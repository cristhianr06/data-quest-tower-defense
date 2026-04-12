// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo
{
    /// <summary>
    /// Modifies the time scale of the game during initialization based on a specified value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TimeScaler"/> class allows for the dynamic adjustment of the game's time scale during initialization. 
    /// Time scale determines the speed at which time progresses in the game, affecting animations, physics, and other 
    /// time-dependent processes.
    /// </para>
    /// </remarks>
    public class TimeScaler : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the time scale of the game.
        /// </summary>
        /// <value>The time scale value. Default is 1.0f representing normal, real-time speed.</value>
        public float TimeScale = 1.0f;

        /// <summary>
        /// Sets the initial time scale of the game during object initialization.
        /// </summary>
        private void Start()
        {
            Time.timeScale = this.TimeScale;
        }
    }
}