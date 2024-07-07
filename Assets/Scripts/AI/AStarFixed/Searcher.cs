using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static Unity.VisualScripting.Member;


//https://www.youtube.com/watch?v=ySN5Wnu88nE
public class Searcher
{

    PriorityQueue PriorityQueue;
    ClosedDictionary ClosedDictionary;
    List<AStarCellWrapper> ExploredCells;

    public Searcher()
    {
        this.PriorityQueue = new PriorityQueue();
        this.ExploredCells = new List<AStarCellWrapper>();
        this.ClosedDictionary = new ClosedDictionary();

    }

    public List<Cell> RefreshPath(Cell start, Cell end)
    {
        //Reset values
        //Hopefully the garbage collector doesn't fuck us over because we're losing so many references hahaha
        this.PriorityQueue = new PriorityQueue();
        this.ExploredCells = new List<AStarCellWrapper>();
        this.ClosedDictionary = new ClosedDictionary();

        //Do the search
        GeneratePath(start, end);
        List<AStarCellWrapper> path = BacktrackPath(this.ClosedDictionary.LookupCell(end));

        //Unwrap the cells
        List<Cell> retList = new List<Cell>();
        
        foreach (AStarCellWrapper wrappedCell in path)
        {
            retList.Add(wrappedCell.Cell);
        }

        retList.Reverse(); //we find the path working backwards from the end... this lets us start from the beginning

        //Debug... Comment this out when not using
        //VisualizeUnwrappedPath(start, retList);

        return retList;
    }

    private void VisualizeUnwrappedPath(Cell start, List<Cell> unwrappedPath)
    {
        //Start is never included in the path
        AStarCellWrapper wrappedStart = this.ClosedDictionary.LookupCell(start);
        wrappedStart.Cell.GeneratePrimitiveAtOrigin(new Color(0, 0, 1));

        //Only visualize the first N nodes (make sure the order is correct)
        int nodesToSee = int.MaxValue;

        //Does not visualize explored but not selected nodes
        foreach (Cell cell in unwrappedPath)
        {

            AStarCellWrapper wrappedCell = this.ClosedDictionary.LookupCell(cell);

            if (wrappedCell.End)
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(1, 0, 0));
            }
            else
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(0, 1, 0));
            }

            if (--nodesToSee == 0)
            {
                break;
            }

        }
    }

    public void Visualize(Cell start, Cell end)
    {
        GeneratePath(start, end);

        foreach (AStarCellWrapper wrappedCell in this.ExploredCells)
        {
            if (wrappedCell.Start)
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(0, 0, 1));
            }
            else if (wrappedCell.End)
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(1, 0, 0));
            }
            else
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(0, 1, 1));
            }
        }

        List<AStarCellWrapper> path = BacktrackPath(this.ClosedDictionary.LookupCell(end));

        foreach (AStarCellWrapper wrappedCell in path)
        {
            if (wrappedCell.Start)
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(0, 0, 1));
            }
            else if (wrappedCell.End)
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(1, 0, 0));
            }
            else
            {
                wrappedCell.Cell.GeneratePrimitiveAtOrigin(new Color(0, 1, 0));
            }
        }

    }

    private void GeneratePath(Cell start, Cell end)
    {
        AStarCellWrapper wrappedStart = ClosedDictionary.RecordOrLookupCell(start);
        wrappedStart.SetStart();
        ExploreCell(wrappedStart, end);
        this.PriorityQueue.GetNext(); //the start cell is added into the pqueue - throw it out!
        AStarCellWrapper wrappedEnd = ClosedDictionary.RecordOrLookupCell(end);
        wrappedEnd.SetEnd();
        //wrappedEnd.Cell.GeneratePrimitiveAtOrigin(new Color(1, 0, 0));

        while (this.PriorityQueue.HasNext())
        {
            AStarCellWrapper bestCell = this.PriorityQueue.GetNext();
            ExploreCell(bestCell, end);



            if (bestCell.End)
            {
                //we've reached the end
                break;
            }

        



        }

    }

    private void ExploreCell(AStarCellWrapper source, Cell target)
    {
        Cell[] Neighbors = source.Cell.getNeighbors();
        this.ExploredCells.Add(source);


        foreach (Cell myNeighbor in Neighbors)
        {
            AStarCellWrapper wrappedCell = ClosedDictionary.RecordOrLookupCell(myNeighbor);
                

            //Ideally, this guard will never fire
            if (wrappedCell == null)
            {
                continue;
            }

            wrappedCell.CalculateHeuristic(target, source);

            this.PriorityQueue.TryAdd(wrappedCell);

        }


        this.PriorityQueue.Sort();



    }

    private List<AStarCellWrapper> BacktrackPath(AStarCellWrapper end)
    {
        List<AStarCellWrapper> path = new List<AStarCellWrapper>();

        AStarCellWrapper currentCell = end;

        while (currentCell.Start == false)
        {

            //Guard for overflow bugs
            if (path.Count > 100)
            {
                break;
            }

            path.Add(currentCell);
            currentCell = currentCell.Path;
        }

        path.Remove(end);

        return path;



    }



 





}
