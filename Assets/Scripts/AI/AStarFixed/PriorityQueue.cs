using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PriorityQueue
{

    private readonly List<AStarCellWrapper> ExploredAStarWrappers;
    private readonly HashSet<AStarCellWrapper> ExploredCellsSet;

    public PriorityQueue()
    {
        this.ExploredAStarWrappers = new List<AStarCellWrapper>();
        this.ExploredCellsSet = new HashSet<AStarCellWrapper>();
    }

    public void TryAdd(AStarCellWrapper cell)
    {
        //We only want something to happen if we have not added this cell to the queue yet
        //If it is already added and a change happened to its heuristic, it will get updated
        //when the queue is sorted at the end of exploration
        //Debug.Log("Tryadd");
        if (this.ExploredCellsSet.Contains(cell) == false)
        {
            //Debug.Log("Adding a cell to the pq");
            ExploredCellsSet.Add(cell);
            ExploredAStarWrappers.Add(cell);
        }
        else
        {
            //Debug.Log("Cell was NOT ADDED to the pq");

        }

    }

    public void Sort()
    {
        //Sort by score
        if (this.ExploredAStarWrappers.Count > 1)
        {
            ExploredAStarWrappers.Sort(delegate (AStarCellWrapper a, AStarCellWrapper b)
            {
                //a and b should never be equivalent... Mathf.sign will never return 0... this shouldn't be an issue? 
                return (int)Mathf.Sign(a.Heuristic - b.Heuristic);
            });
        }

    }

    public bool HasNext()
    {
        //We avoid adding this back because we do not remove this element from the explored cell set
        return ExploredAStarWrappers.Count > 0;
    }

    public AStarCellWrapper GetNext()
    {
        AStarCellWrapper ret = this.ExploredAStarWrappers[0];
        this.ExploredAStarWrappers.Remove(ret);
        return ret;
    }

}
