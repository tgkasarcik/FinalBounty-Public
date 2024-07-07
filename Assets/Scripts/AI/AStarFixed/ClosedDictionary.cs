using PlanetGen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedDictionary
{

    private readonly Dictionary<int, AStarCellWrapper> Dictionary;

    public ClosedDictionary()
    {
        this.Dictionary = new Dictionary<int, AStarCellWrapper>();
    }

    public AStarCellWrapper RecordOrLookupCell(Cell cell)
    {
        AStarCellWrapper cellWrapper = new(cell);
        this.Dictionary.TryAdd(cell.GetIdentifier(), cellWrapper);

        /*TryAdd will fail if we already have a cellWrapper under the cell key
          This will make sure we ONLY return what is in the dicitonary
          (although this should never happen)*/
        return LookupCell(cell);
    }

    public AStarCellWrapper LookupCell(Cell cell)
    {
        //Return the hit or return null
        return this.Dictionary.TryGetValue(cell.GetIdentifier(), out AStarCellWrapper wrappedCell) ? wrappedCell : null;
    }







}
