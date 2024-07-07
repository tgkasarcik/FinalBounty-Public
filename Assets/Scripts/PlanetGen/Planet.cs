using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace PlanetGen
{
    public class Planet
    {
        private Vector3 scale;
        [SerializeField, HideInInspector]
        MeshFilter[] meshFilters;
        [SerializeField, HideInInspector]
        private PlanetFace[] planetFaces;
        GameObject parent;
        public GameObject meshObject;
        Mesh mesh;
        private int[] rectangles;
        // https://www.youtube.com/watch?v=QN39W020LqU&t=521s https://github.com/SebLague/Procedural-Planets/blob/master/Procedural%20Planet%20E01/Assets/Planet.cs
        public Planet(GameObject parent, int resolution, Vector3 scale)
        {
            this.scale = scale;
            this.parent = parent;
            if (meshFilters == null || meshFilters.Length == 0)
            {
                meshFilters = new MeshFilter[6];
            }
            planetFaces = new PlanetFace[6];
            
            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

            //float scaleVal = this.parent.transform.localScale.x / 2 + 1;
            float scaleVal = 1;
            for (int i = 0; i < planetFaces.Length; i++)
            {
                if (meshFilters[i] == null)
                {
                    GameObject meshObject = new GameObject("mesh");
                    meshObject.transform.parent = parent.transform;
                    meshObject.transform.localPosition = Vector3.zero;
                    meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();

                }
                planetFaces[i] = new PlanetFace(meshFilters[i].sharedMesh, resolution, directions[i], scaleVal);

            }

        }
        
        public PlanetFace[] getFaces()
        {
            return planetFaces;
        }
        public void GenerateMesh()
        {

            foreach (PlanetFace face in planetFaces)
            {

                face.ContructVertices();

            }

            
            CombineMeshes();
            
            CreateRectangles(meshObject.GetComponent<MeshFilter>().sharedMesh.triangles);
        }
        //https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
        private void CombineMeshes() 
        {
            CombineInstance[] meshCombine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < 6; i++)
            {
                var filter = meshFilters[i];
                meshCombine[i].mesh = meshFilters[i].sharedMesh;
                meshCombine[i].transform = filter.transform.localToWorldMatrix;
                filter.gameObject.SetActive(false);
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(meshCombine);
            combinedMesh.RecalculateTangents();
            RemoveDuplicates(combinedMesh);

            combinedMesh.RecalculateNormals();

            meshObject = new GameObject("TOTAL");
            meshObject.AddComponent<MeshRenderer>();/*.sharedMaterial = new Material(Shader.Find("Standard"));*/
            meshObject.AddComponent<SphereCollider>();
            meshObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
            meshObject.transform.parent = parent.transform;
            meshObject.transform.localScale = scale;
            mesh = meshObject.GetComponent<MeshFilter>().mesh;
            meshObject.SetActive(false);
            //meshObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color32(245, 243, 193, 1);
        }
        private void RemoveDuplicates(Mesh mesh)
        {
            var oldtriangles = mesh.triangles;
            int[] newTriangles = new int[mesh.triangles.Length];
            var oldVertices = mesh.vertices;
            List<Vector3> vertices = oldVertices.Distinct().ToList();
            Vector3[] newVertices = vertices.ToArray();

            for (int i = 0; i < oldtriangles.Length; i++)
            {
                int tri = oldtriangles[i];
                Vector3 value = oldVertices[tri];
                newTriangles[i] = vertices.IndexOf(value);

            }
            mesh.triangles = newTriangles;
            mesh.vertices = newVertices;
        }
        

        private void CreateRectangles(int[] triangles)
        {
            int length = (triangles.Length / 6) * 4;
            rectangles = new int[length];
            int rectIndex = 0;
            for (int i = 0; i < triangles.Length; i += 6)
            {
                rectangles[rectIndex] = triangles[i];
                rectangles[rectIndex + 1] = triangles[i + 4];
                rectangles[rectIndex + 2] = triangles[i+2];
                rectangles[rectIndex + 3] = triangles[i + 1];
                rectIndex += 4;
            }

        }
        public int[] getRectangles()
        {
            return rectangles;
        }
        public Vector3[] getVertices()
        {
            return meshObject.GetComponent<MeshFilter>().sharedMesh.vertices;
        }
        
        
    }
}
