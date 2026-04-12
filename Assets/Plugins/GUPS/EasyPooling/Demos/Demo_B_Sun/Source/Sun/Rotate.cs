// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.B
{
    /// <summary>
    /// Provides functionality to rotate a GameObject (in this demo the sun) continuously based on a specified speed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Rotate"/> class, offers a convenient way to implement continuous rotation for a GameObject within 
    /// a Unity scene. By adjusting the <see cref="Speed"/> property, clients can control the rotation speed of the 
    /// associated GameObject around its local z-axis. The rotation occurs in the <see cref="Update"/> method.
    /// </para>
    /// </remarks>
    public class Rotate : MonoBehaviour
    {
        /// <summary>
        /// The speed at which the GameObject rotates around its local z-axis.
        /// </summary>
        [Tooltip("The speed of rotation around the local z-axis.")]
        public float Speed = 5f;

        /// <summary>
        /// Called once per frame to update the rotation of the GameObject.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Update"/> method is responsible for continuously rotating the associated GameObject based on the
        /// specified rotation speed. The rotation occurs around the local z-axis, providing a simple and effective way to
        /// add dynamic movement to GameObjects.
        /// </para>
        /// </remarks>
        protected virtual void Update()
        {
            // Rotate the game object around its local z-axis.
            this.transform.Rotate(0f, 0f, this.Speed * Time.deltaTime);
        }
    }
}