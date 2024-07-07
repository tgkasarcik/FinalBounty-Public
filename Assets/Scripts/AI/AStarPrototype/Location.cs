using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location 
{

    public Cell MyCell { get; }
    private Location LastLocation;
    private Cell Goal;

    private LocationContainer ParentContainer;

    public Location PreviousLocation;


    private Vector3 Coordinate;
    public float DistanceFromStart;
    public float DistanceFromGoal;
    public float Score { get; } //score == cost

    //Many A* searches may run simultaneously. This requires unique location elements for each searcher
    //These 
    public Location(Cell currentCell, Location lastLocation, Cell goal, LocationContainer parentContainer)
    {
        this.MyCell = currentCell;
        this.LastLocation = lastLocation;
        this.Goal = goal;

        this.Coordinate = currentCell.getLocation();
        this.DistanceFromStart = currentCell.DistanceToCell(lastLocation.MyCell) + lastLocation.DistanceFromStart; //not an estimate, this is exact
        this.DistanceFromGoal = currentCell.DistanceToCell(goal); //just an estimate
        this.Score = (Mathf.Abs(DistanceFromStart) + Mathf.Abs(DistanceFromGoal));
        this.ParentContainer = parentContainer;
    }

    //Overloaded for the starting location
    public Location(Cell currentCell, Cell goal, LocationContainer parentContainer)
    {
        this.MyCell = currentCell;
        this.LastLocation = this;
        this.Goal = goal;

        this.Coordinate = currentCell.getLocation();
        this.DistanceFromStart = 0;
        this.DistanceFromGoal = currentCell.DistanceToCell(goal);
        this.Score = (Mathf.Abs(DistanceFromStart) + Mathf.Abs(DistanceFromGoal));
        this.ParentContainer = parentContainer;
    }

    public bool AlternateDistanceFromStart(Location differentLastLocation)
    {
        float calc = this.MyCell.DistanceToCell(differentLastLocation.MyCell) + differentLastLocation.DistanceFromStart;

        if (calc < DistanceFromStart)
        {
            Debug.Log("We found a shorter distance!");
            this.DistanceFromStart = calc;
            return true;
        }

        return false;

    }

    public List<Location> GetNeighboringLocations()
    {
        Cell[] cellArr = MyCell.getNeighbors();
        List<Location> ret = new(); //shorthand constructor

        for (int i = 0; i < cellArr.Length; i++)
        {
            Cell cell = cellArr[i];
            ret.Add(this.ParentContainer.GetLocationFromCell(cell, this));
        }

        return ret;
    }
    override public string ToString()
    {
        return "Location is " + Coordinate.ToString() + " Score is: " + Score;
    }


}
