// Unity
using UnityEngine;

namespace GUPS.EasyPooling.Demo.A
{
    /// <summary>
    /// Generates a 3D volumetric asteroid mesh with noise for irregularity.
    /// </summary>
    public class AsteroidMeshGenerator : MonoBehaviour
    {
        /// <summary>
        /// Number of segments in the asteroid mesh.
        /// </summary>
        public int segments = 10;

        /// <summary>
        /// Radius of the asteroid.
        /// </summary>
        public float radius = 5f;

        /// <summary>
        /// Minimum value for noise applied to vertices.
        /// </summary>
        public float minNoise = -1f;

        /// <summary>
        /// Maximum value for noise applied to vertices.
        /// </summary>
        public float maxNoise = 1f;

        /// <summary>
        /// Generates the asteroid mesh on start.
        /// </summary>
        protected virtual void Start()
        {
            this.GenerateAsteroidMesh();
        }

        /// <summary>
        /// Generates the 3D volumetric asteroid mesh.
        /// </summary>
        private void GenerateAsteroidMesh()
        {
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            int vertexCount = (segments + 1) * (segments + 1);
            int triangleCount = segments * segments * 6;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[triangleCount * 3];
            Vector2[] uvs = new Vector2[vertexCount];

            int index = 0;

            for (int i = 0; i <= segments; i++)
            {
                float phi = Mathf.PI * i / segments;
                for (int j = 0; j <= segments; j++)
                {
                    float theta = 2 * Mathf.PI * j / segments;

                    float x = Mathf.Sin(phi) * Mathf.Cos(theta) * radius;
                    float y = Mathf.Sin(phi) * Mathf.Sin(theta) * radius;
                    float z = Mathf.Cos(phi) * radius;

                    // Apply noise for irregularity
                    float noise = Random.Range(minNoise, maxNoise);
                    x += noise;
                    y += noise;
                    z += noise;

                    vertices[index] = new Vector3(x, y, z);
                    uvs[index] = new Vector2((float)j / segments, (float)i / segments);
                    index++;
                }
            }

            index = 0;
            for (int i = 0; i < segments; i++)
            {
                for (int j = 0; j < segments; j++)
                {
                    int nextRow = (segments + 1);

                    int a = index;
                    int b = index + 1;
                    int c = index + nextRow;
                    int d = index + nextRow + 1;

                    triangles[index * 6] = a;
                    triangles[index * 6 + 1] = c;
                    triangles[index * 6 + 2] = b;
                    triangles[index * 6 + 3] = b;
                    triangles[index * 6 + 4] = c;
                    triangles[index * 6 + 5] = d;

                    index++;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}