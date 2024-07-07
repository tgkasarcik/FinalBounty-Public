using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

namespace PlanetGen
{
    public class Cell
    {

        public float VisitCost;

        public Vector3 center;
        private Boolean filled;
        public Vector3[] corners;
        private Cell[] neighbors;
        private int[] neighborIndexes;
        private int index;
        public GameObject parent;
        private Vector3 scale;
        private GameObject collider;
        private GameObject SphereVisulizer;

        public bool StartFlag; //THIS IS A TEMPORARY SOLUTION TO THE BACKTRACKING MISFINDING THE START NODE
        //TODO: Find a more permanent solution (that will allow this cell to be used as the target for several agents)

        public Cell(Vector3[] corners, int[] neighborIndexes, GameObject parent, Vector3 scale, int index)
        {
            this.corners = corners;
            this.index= index;
            this.filled = false;
            this.neighborIndexes = neighborIndexes;
            this.neighbors = new Cell[8];
            this.parent = parent;
            this.scale = scale/2;
            findCenter();
            loadInCollider();

            //TODO: Replace this with real cost. A good system for this will involve some trial and error.
            //Normal cells should probably be random values in small range (e.g. [0, 4]) to promote non-linear pathfinding
            //More expensive cells should be in a small range of higher values (e.g. [10, 14])
            System.Random rand = new System.Random();
            this.VisitCost = 5f + rand.Next(15);

        }
        public void findNeighbors(Cell[] cells)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                neighbors[i] = cells[neighborIndexes[i]];
            }

        }

        public int GetIdentifier()
        {
            return this.index;
        }

        public Vector3 getLocation() { return this.center; }
        public Cell[] getNeighbors()
        {
            return this.neighbors;
        }
        public Boolean IsFilled()
        {
            return filled;
        }
        public void SetFilled(Boolean filled)
        {
            this.filled = filled;
        }
        private void findCenter()
        {
            
            this.center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
            this.center.Normalize();
            this.center.Scale(this.scale);
        }
        private void loadInCollider()
        {
            collider = new GameObject("Grid Collider");
            //Ignores the raycast for the boss level
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            collider.layer = LayerIgnoreRaycast;
            collider.transform.parent = parent.transform;
            var p1 = corners[0]; var p2 = corners[1];
            p1.Scale(this.scale);
            p2.Scale(this.scale);
            float width = Mathf.Sqrt(((p1.x - p2.x) * (p1.x - p2.x)) + ((p1.y - p2.y) * (p1.y - p2.y)) + ((p1.z - p2.z) * (p1.z - p2.z)))*1.05f;
            collider.AddComponent<BoxCollider>().size = new Vector3(width,width * 3f,width);
            collider.transform.position = this.center;
            collider.transform.position = Vector3.MoveTowards(collider.transform.position, parent.transform.position, .5f);
            Vector3 grav = (collider.transform.position - parent.transform.position).normalized;
            collider.transform.rotation = Quaternion.FromToRotation(collider.transform.up, grav) * collider.transform.rotation;
            var box = collider.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.tag = "Cell";
            //This script just stores a tag
            box.AddComponent<TriggerString>().SetTag(index.ToString());
            collider.name = "Grid Collider" + index.ToString();



        }
        public String getColliderTag()
        {
            return index.ToString();
        }

        //TODO: This is inefficient. Find a better solution for the radius like calculating it upon cell creation or some shit idk
        //I'm convinced this works, but I havent tested it thoroughly
        public float DistanceToCell(Cell targetCell)
        {
            Vector3 targetLocation = targetCell.getLocation();
            Debug.Log(targetLocation.x + " "  + targetLocation.y + " "  + targetLocation.z);
            float radiusOfSphere =  Vector3.Magnitude(center - parent.transform.position);
            Debug.Log(radiusOfSphere);

            //https://math.stackexchange.com/questions/1304169/distance-between-two-points-on-a-sphere
            float distanceBetweenCells = radiusOfSphere * math.acos(Vector3.Dot(center, targetLocation) / (radiusOfSphere * radiusOfSphere));
            Debug.Log(distanceBetweenCells);
            return distanceBetweenCells;
        }

        public float GetSphereRadius()
        {
            return Vector3.Magnitude(center - parent.transform.position);
        }

        public void GeneratePrimitiveAtOrigin(Color color)
        {

            if (SphereVisulizer != null)
            {
                GameObject.Destroy(SphereVisulizer.gameObject);
            }

            SphereVisulizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SphereVisulizer.transform.position = this.center;
            SphereVisulizer.GetComponent<Renderer>().material.color = color;
            SphereVisulizer.name = "Debug Sphere :)";
            SphereVisulizer.transform.localScale = new Vector3(2, 2, 2);
            SphereVisulizer.GetComponent<SphereCollider>().enabled = false;
        }
    }
    public class GridSystem
    {
        private Vector3[] Vertices;
        private int[] Rectangles;
        private int resolution;
        private Cell[] cells;
        private GameObject parent;
        private Vector3 scale;
        public GridSystem(int resolution, int[] Rectangles, Vector3[] Vertices, GameObject parent, Vector3 scale)
        {
            this.resolution = resolution;
            this.Vertices = Vertices;
            this.Rectangles = Rectangles;
            this.cells = new Cell[Rectangles.Length/4];
            this.parent = parent;
            this.scale = scale;//new Vector3(scale.x / parent.transform.localScale.x, scale.y / parent.transform.localScale.y, scale.z / parent.transform.localScale.z);
        }
        
        public void GenerateCells()
        {
            Neighbors neighborClass = new Neighbors(Vertices, Rectangles);
            for (int i = 0; i < Rectangles.Length/4; i++)
            {
                Vector3[] corners = new Vector3[] { Vertices[Rectangles[i * 4]], Vertices[Rectangles[i * 4 + 1]],
                    Vertices[Rectangles[i * 4 + 2]], Vertices[Rectangles[i * 4 + 3]] };
                foreach (Vector3 c in corners)
                {
                    c.Scale(scale);
                }
                int[] neighbors = neighborClass.getNeighbors(i);
                
                cells[i] = new Cell(corners, neighbors, parent, scale, i);

            }

            for (int j = 0; j<cells.Length; j++)
            {
                cells[j].findNeighbors(cells);
            }

        }

        public Cell[] GetCells()
        {
            return cells;
        }



    }
    

}


