using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=QN39W020LqU&t=521s https://github.com/SebLague/Procedural-Planets/blob/master/Procedural%20Planet%20E01/Assets/TerrainFace.cs
namespace PlanetGen
{
    public class PlanetFace
    {
        //TODO remove excess calculations and only generate vertices and normals

        //number of vertices along a single edge
        int resolution;
        Vector3 localUp;
        Vector3 axisA;
        Vector3 axisB;
        private float scaleVal;
        private Vector3[] vertices;
        Mesh mesh;

        public PlanetFace(Mesh mesh, int resolution, Vector3 localUp, float scaleVal)
        {
            this.mesh = mesh;
            this.resolution = resolution;
            this.localUp = localUp;
            this.scaleVal = scaleVal;

            axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            axisB = Vector3.Cross(localUp, axisA);

        }
        public void ContructVertices()
        {
            float s = 10000;
            vertices = new Vector3[resolution * resolution];
            //loops through all of the possible vector coordinates of one planetface
            Vector3 scale = new(this.scaleVal, this.scaleVal, this.scaleVal);

            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];
            //loops through all of the possible coordinates of one meshface
            int triIndex = 0;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + y * resolution;
                    //percent determines where in the grid system the coordinate will be
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointsOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;

                    Vector3 pointsOnUnitSphere = CubeToSphere(pointsOnUnitCube);
                    vertices[i] = pointsOnUnitSphere;
                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;

                        triIndex += 6;

                    }

                }
            }
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.normals = vertices;

        }

        private Vector3 CubeToSphere(Vector3 point)
        {
            float x, y, z;
            x = point.x; y = point.y; z = point.z;
            float a, b, c;
            a = x * Mathf.Sqrt(1f - (y * y + z * z) / 2f + (y * y * z * z) / 3);
            b = y * Mathf.Sqrt(1f - (x * x + z * z) / 2f + (x * x * z * z) / 3);
            c = z * Mathf.Sqrt(1f - (y * y + x * x) / 2f + (y * y * x * x) / 3);
            return new Vector3(a, b, c);
        }
        public Vector3[] getVertices()
        {
            return vertices;
        }

        public Mesh getMesh()
        {
            return mesh;
        }

    }
}
