using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class AStarCellWrapper
{

    public Cell Cell;
    public float VisitCost; //the cost of visiting the node: e.g. a hurricane may be 10 while clear skies are 1
    public float Heuristic; //The cost of visiting plus the distance to the goal
    public AStarCellWrapper Path; //The node which gives us the cheapest traversal cost goes here
    public bool Start;
    public bool End;

    public AStarCellWrapper(Cell myCell)
    {

        System.Random rand = new System.Random();

        this.Cell = myCell;
        this.VisitCost = myCell.VisitCost; //defined in GridSystem.cs to preserve values each time a path is found

        this.Heuristic = float.MaxValue;
        this.Start = false;
        this.End = false;



    }

    public void SetStart()
    {
        this.Start = true;
        this.VisitCost = 0;
    }

    public void SetEnd()
    {
        this.End = true;
        this.VisitCost = -99999; //float.minValue gives some weird overflow errors in the pqueue
    }

    public void CalculateHeuristic(Cell end, AStarCellWrapper from)
    {
        Vector3 myLocation = Cell.getLocation();
        Vector3 endLocation = end.getLocation();
        float radiusOfPlanet = Cell.GetSphereRadius();
        float distanceCost = radiusOfPlanet * math.acos(Vector3.Dot(myLocation, endLocation) / (radiusOfPlanet * radiusOfPlanet));
        float heuristic = distanceCost + this.VisitCost;

        //If the cell we are coming from gives us the shortest path, update it!
       if (this.End)
        {
            this.Heuristic = 0;
            this.Path = from;

        }
        else if (heuristic < this.Heuristic)
        {
            this.Heuristic = heuristic;
            this.Path = from;
        }
        else if (math.abs(heuristic - this.Heuristic) < 0.001)
        {
            //Debug.Log("This check actually works. new: " + heuristic + " OG: " + this.Heuristic);
           
        }



    }




}
