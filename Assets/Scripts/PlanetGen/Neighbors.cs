using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace PlanetGen
{
    public class Neighbors
    {
        Vector3[] vertices;
        int[] rectangles;

        Dictionary<Vector3, List<int>> corners = new Dictionary<Vector3, List<int>>();
        public Neighbors(Vector3[] vertices, int[] rectangles)
        {
            this.vertices = vertices;
            this.rectangles = rectangles;
            FindNeighbors();
        }

        private void FindNeighbors()
        {
            corners = new Dictionary<Vector3, List<int>>();
            for (int i = 0; i < rectangles.Length; i += 4)
            {
                AddToDict(vertices[rectangles[i]], i / 4);
                AddToDict(vertices[rectangles[i + 1]], i / 4);
                AddToDict(vertices[rectangles[i + 2]], i / 4);
                AddToDict(vertices[rectangles[i + 3]], i / 4);
            }

        }
        public void AddToDict(Vector3 corner, int val)
        {
            if (corners.ContainsKey(corner))
            {
                corners[corner].Add(val);
            }
            else
            {
                corners.Add(corner, new List<int>());
                corners[corner].Add(val);
            }
        }
        public int[] getNeighbors(int sqrIndex)
        {

            int[] neighbors = new int[8];
            var sqrList = corners[vertices[rectangles[4 * sqrIndex]]];
            int neighborIndex = 0;
            for ( int j= 0; j < 4; j++)
            {
                
                List<int> list = corners[vertices[rectangles[4 * sqrIndex + j]]];
                for (int k = 0; k < list.Count; k++)
                {
                    int curr = list[k];
                    if (!neighbors.Contains(curr) && curr != sqrIndex)
                    {
                        //Debug.Log(neighborIndex);
                        neighbors[neighborIndex] = curr;
                        neighborIndex++;
                    }
                }
            }
            return neighbors;
        }
    }
    public struct Edge
    {
        public Vector3 v1;
        public Vector3 v2;

        public Edge(Vector3 v1, Vector3 v2)
        {
            if (v1.GetHashCode() > v2.GetHashCode())
            {
                this.v1 = v1;
                this.v2 = v2;
            }
            else
            {
                this.v1 = v2;
                this.v2 = v1;
            }

        }
    }
}
