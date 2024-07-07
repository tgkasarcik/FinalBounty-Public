using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetGen;
using Unity.Mathematics;
using System.Security.Cryptography.X509Certificates;

public class Visualizer : MonoBehaviour
{
    public GenPlanet MyPlanet;

    private OpenList openList;
    private ClosedList closedList;
    private LocationContainer locationContainer;


    private void ProcessLocation(Location loc, Location prev)
    {
        closedList.AddLocation(loc);
        List<Location> neighbors = loc.GetNeighboringLocations();
        foreach (Location location in neighbors)
        {
            if (openList.Contains(location)) //This could be done internally inside the open list
            {
                if (location.AlternateDistanceFromStart(loc)) //See if we found a shorter path to this node 
                {
                    openList.Sort();
                    location.PreviousLocation = prev;
                }
            }
            else if (false == closedList.Contains(location))
            {
                openList.AddLocation(location);
                //location.PreviousLocation = prev;

            }
        }

    }

    private Location BackTrackToNextNodeCheating(Location current)
    {
        List<Location> neighbors = current.GetNeighboringLocations();
        List<Location> eligibleNeighbors = new List<Location>();

        foreach (Location location in neighbors)
        {
            if (location.MyCell.StartFlag == false && closedList.Contains(location))
            {
                eligibleNeighbors.Add(location);
            }
        }

        if (eligibleNeighbors.Count > 0)
        {


            Location cheapestLocation = eligibleNeighbors[0];

            foreach (Location location in eligibleNeighbors)
            {

                if (location.DistanceFromStart < cheapestLocation.DistanceFromStart)
                {
                    cheapestLocation = location;
                }
            }

            closedList.Remove(cheapestLocation);

            Debug.Log("NumNeighbors: " + neighbors.Count + " num eligible " + eligibleNeighbors.Count);
            return cheapestLocation;
        }
        return null;
    }

    private Location BackTrackToNextNode(Location current) //rememebr, start is where we want to end up when backtracking!
    {
        List<Location> neighbors = current.GetNeighboringLocations();
        List<Location> eligibleNeighbors = new List<Location>();

        foreach (Location location in neighbors)
        {
            if (location.MyCell.StartFlag == false && closedList.Contains(location))
            {
                eligibleNeighbors.Add(location);
            }
        }

        Location cheapestLocation = eligibleNeighbors[0];

        foreach (Location location in eligibleNeighbors)
        {
            if (location.Score < cheapestLocation.Score)
            {
                cheapestLocation = location;
            }
        }

        closedList.Remove(cheapestLocation);

        Debug.Log("NumNeighbors: " + neighbors.Count + " num eligible " + eligibleNeighbors.Count);
        return cheapestLocation;


    }



    private void Start()
    {
        System.Random rand = new System.Random();
        Debug.Log("We've starrrrted");
        Cell[] cells = MyPlanet.Grid.GetCells();
        int randNum = rand.Next(cells.Length - 1);
        Cell endCell = cells[190];
        Debug.Log("Randnum is: " + randNum);
        //Cell endCell = cells[cells.Length - 1];
        Cell startCell = cells[0];
        startCell.StartFlag = true;

        //Path from the start to last cell
        locationContainer = new LocationContainer(startCell, endCell);
        Location startLocation = locationContainer.GetLocationFromCell(startCell, new Location(startCell, endCell, locationContainer)); //Make the first cell a location stored in the container

        startCell.GeneratePrimitiveAtOrigin(new Color(0, 0, 1));
        endCell.GeneratePrimitiveAtOrigin(new Color(0, 0, 0));

        openList = new OpenList();
        closedList = new ClosedList();

        ProcessLocation(startLocation, null);

        Location localBest = startLocation;
        Location previousLocalBest = localBest;
        while (false == localBest.MyCell.getLocation().Equals(endCell.getLocation()))
        {
            localBest = openList.GetBestLocation();

            if (localBest.PreviousLocation != null)
            {
                Debug.LogWarning("This already has a parent");

                //if (localBest.PreviousLocation.Score > previousLocalBest.Score)
                //{
                //    Debug.Log("Trying to fix....");
                //    localBest.PreviousLocation = previousLocalBest;
                //}

            } 
            
            localBest.PreviousLocation = previousLocalBest;

            

            closedList.AddLocation(localBest);
            ProcessLocation(localBest, previousLocalBest);
            localBest.MyCell.GeneratePrimitiveAtOrigin(new Color(1, 1, 1));
       


            previousLocalBest = localBest;
        }


        Location backtrackedNode = localBest; //this is the end cell
        //int sentinal = 0;
        //while (backtrackedNode.PreviousLocation != null)
        //{
        //    backtrackedNode.MyCell.GeneratePrimitiveAtOrigin(new Color(1, 1, 0));
        //    backtrackedNode = backtrackedNode.PreviousLocation;
        //    sentinal++;
        //}


        int sentinal = 0;

        while (backtrackedNode != null && false == backtrackedNode.MyCell.StartFlag && sentinal < 100)
        {
            backtrackedNode.MyCell.GeneratePrimitiveAtOrigin(new Color(1, 1, 0));
            backtrackedNode = BackTrackToNextNodeCheating(backtrackedNode);
            sentinal++;
        }




        endCell.GeneratePrimitiveAtOrigin(new Color(0, 1, 1)); //signal that we found the exit!

        //startCell.GeneratePrimitiveAtOrigin(new Color(0, 0, 1));






        //Debug.Log("Length is: " + cells.Length);
        //Debug.Log("Positions are: ");

        //LocationContainer LocCon = new(cells[0], cells[cells.Length - 1]);
        //System.Random rand = new System.Random();

        //cells[0].GeneratePrimitiveAtOrigin(new Color(0, 0, 1)); //starting cell
        //cells[rand.Next(cells.Length - 1)].GeneratePrimitiveAtOrigin(new Color(0, 1, 0)); //unsure if random is inclusive or not.... anyways it doesn't matter for a prototype

        //Cell randCell = cells[rand.Next(cells.Length - 1)];
        //randCell.GeneratePrimitiveAtOrigin(new Color(0, 0, 0));
        //List<Location> locList = LocCon.GetNeighbors(randCell);

        //OpenList openList = new OpenList();
        ////openList.AddLocation(LocCon.GetLocationFromCell(randCell));

        //foreach (Location loc in locList )
        //{
        //    loc.MyCell.GeneratePrimitiveAtOrigin(new Color(1, 1, 1));
        //    openList.AddLocation(loc);
        //}

        //Debug.Log("Open list is:\n");
        //openList.PrintList();
        //Debug.Log("Len: " + openList.getLen());

        //Debug.Log("The best location is: ");
        //Location bestLoc = openList.GetBestLocation();
        //Debug.Log(bestLoc.ToString());

        //Debug.Log("Open list is:\n");
        //openList.PrintList();
        //Debug.Log("Len: " + openList.getLen());

        //bestLoc.MyCell.GeneratePrimitiveAtOrigin(new Color(1, 0, 1));



        //for (int i = 0; i < 1; i++)
        //{
        //    Cell cell = cells[i];


        //    Debug.Log("Cell " + i + ": " + cell.getLocation().ToString() + "Neighbors are: ");

        //    //alpha is 0: transparent -> 255: opaque
        //    cell.GeneratePrimitiveAtOrigin(new Color(0, 1, 1, 50));

        //    Cell[] neighbors = cell.getNeighbors();
        //    for (int j = 0; j < neighbors.Length; j++)
        //    {
        //        Cell neighbor = neighbors[j];
        //        Debug.Log("Neighbor " + j + ": " + neighbor.getLocation().ToString());
        //        neighbor.GeneratePrimitiveAtOrigin(new Color(1, 1, 0, 50));
        //        Debug.Log(cell.DistanceToCell(neighbor));
        //    }

        //}


    }

}
