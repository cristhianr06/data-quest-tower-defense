using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    /// <summary>
    /// Draws a wiresphere where the backface is a shade lighter.
    /// Matches the standard AudioSource scene gizmo.
    /// </summary>
    static class RadiusHandle
    {
        private const float BackfaceAlphaMultiplier = 0.2f;

        public static void DrawRadiusHandle(Quaternion rotation, Vector3 position, float radius)
        {
            Vector3[] directions =
            {
                rotation * Vector3.right,
                rotation * Vector3.up,
                rotation * Vector3.forward,
            };

            if (Camera.current.orthographic)
            {
                var planeNormal = Camera.current.transform.forward;

                Handles.DrawWireDisc(position, planeNormal, radius);

                for (var i = 0; i < 3; i++)
                {
                    var direction = directions[i];
                    var from = Vector3.Cross(direction, planeNormal).normalized;
                    DrawTwoShadedWireDisc(position, direction, from, 180f, radius);
                }
            }
            else
            {
                // Since the geometry is transformed by Handles.matrix during rendering, we transform the camera position
                // by the inverse matrix so that the two-shaded wireframe will have the proper orientation.
                var invMatrix = Matrix4x4.Inverse(Handles.matrix);

                var planeNormal = position - invMatrix.MultiplyPoint(Camera.current.transform.position); // vector from camera to center
                var sqrDist = planeNormal.sqrMagnitude; // squared distance from camera to center
                var sqrRadius = radius * radius; // squared radius
                var sqrOffset = sqrRadius * sqrRadius / sqrDist; // squared distance from actual center to drawn disc center
                var insideAmount = sqrOffset / sqrRadius;

                // If we are not inside the sphere, calculate where to draw the periphery
                if (insideAmount < 1f)
                {
                    var drawnRadius = Mathf.Sqrt(sqrRadius - sqrOffset); // the radius of the drawn disc

                    // Draw periphery circle
                    Handles.DrawWireDisc(position - sqrRadius * planeNormal / sqrDist, planeNormal, drawnRadius);
                }

                // Draw two-shaded axis-aligned circles
                for (var i = 0; i < 3; i++)
                {
                    if (insideAmount < 1f)
                    {
                        var Q = Vector3.Angle(planeNormal, directions[i]);
                        Q = 90f - Mathf.Min(Q, 180f - Q);
                        var f = Mathf.Tan(Q * Mathf.Deg2Rad);
                        var g = Mathf.Sqrt(sqrOffset + f * f * sqrOffset) / radius;
                        if (g < 1f)
                        {
                            var e = Mathf.Asin(g) * Mathf.Rad2Deg;
                            var from = Vector3.Cross(directions[i], planeNormal).normalized;
                            from = Quaternion.AngleAxis(e, directions[i]) * from;
                            DrawTwoShadedWireDisc(position, directions[i], from, (90f - e) * 2f, radius);
                        }
                        else
                        {
                            DrawTwoShadedWireDisc(position, directions[i], radius);
                        }
                    }
                    else
                    {
                        DrawTwoShadedWireDisc(position, directions[i], radius);
                    }
                }
            }
        }

        private static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, float radius)
        {
            var color = Handles.color;
            var originalColor = color;
            color.a *= BackfaceAlphaMultiplier;

            Handles.color = color;
            Handles.DrawWireDisc(position, axis, radius);
            Handles.color = originalColor;
        }

        private static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, Vector3 from, float degrees, float radius)
        {
            Handles.DrawWireArc(position, axis, from, degrees, radius);

            var color = Handles.color;
            var originalColor = color;
            color.a *= BackfaceAlphaMultiplier;

            Handles.color = color;
            Handles.DrawWireArc(position, axis, from, degrees - 360, radius);
            Handles.color = originalColor;
        }
    }
}
