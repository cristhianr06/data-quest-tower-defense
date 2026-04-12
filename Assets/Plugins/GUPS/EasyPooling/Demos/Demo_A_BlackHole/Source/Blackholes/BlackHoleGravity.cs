// System
using System.Linq;

// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Simulates the gravitational effects of a black hole on nearby celestial bodies within its influence.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="BlackHoleGravity"/> class represents a celestial body with a powerful gravitational field that affects nearby objects.
    /// It applies gravitational forces, alters orbital velocities, and enforces specific behaviors based on the distance from the black hole.
    /// </para>
    /// <para>
    /// The gravitational effects are applied in the <see cref="FixedUpdate"/> method, where the gravitational force, slowing factor, and orbital velocity
    /// are calculated for each affected object. Objects within the minimum distance are dragged straight towards the black hole.
    /// </para>
    /// <para>
    /// It is crucial for celestial bodies affected by the black hole to implement the <see cref="ICelestialBody"/> interface, which defines essential
    /// properties and methods for objects under gravitational influence.
    /// </para>
    /// </remarks>
    public class BlackHoleGravity : MonoBehaviour
    {
        /// <summary>
        /// The maximum distance at which the black hole will start pulling objects towards it.
        /// </summary>
        public float maxDistance = 25f;

        /// <summary>
        /// Called at fixed intervals, applies gravitational effects to nearby celestial bodies.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            GameObject[] objectsAffected = Physics.OverlapSphere(this.transform.position, this.maxDistance)
                .Select(c => c.gameObject)
                .Where(g => g.GetComponents<ICelestialBody>().Length > 0)
                .ToArray();

            foreach (GameObject obj in objectsAffected)
            {
                // Get a vector to the black hole from the object.
                Vector3 directionToBlackHole = transform.position - obj.transform.position;

                // Get the distance from the object to the black hole.
                float distance = directionToBlackHole.magnitude;

                // Rotate the directin vector 90 degrees around the Y axis.
                Vector3 orbitalVelocity = Vector3.Cross(directionToBlackHole.normalized, Vector3.up);

                // Calculate the velocity.
                Vector3 velocity = (directionToBlackHole.normalized / 5f + orbitalVelocity.normalized).normalized * Time.deltaTime * 2.5f;

                // Set y velocity to 0.
                velocity = new Vector3(velocity.x, 0f, velocity.z);

                // Move the object.
                obj.transform.position += velocity;
            }
        }
    }
}