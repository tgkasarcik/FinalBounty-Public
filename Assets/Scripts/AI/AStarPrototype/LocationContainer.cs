using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationContainer 
{

    private readonly Dictionary<Cell, Location> CellToLocationDict;
    private Cell startCell;
    private Cell endCell;

    public LocationContainer(Cell startCell, Cell endCell)
    {
        CellToLocationDict = new Dictionary<Cell, Location>();
        this.startCell = startCell;
        this.endCell = endCell;


    }


    public Location GetLocationFromCell(Cell cell, Location currentLocation) 
    {
        //Inline variable declaration for ret 
        //If we haven't processed the cell into a location yet, do it
        if (false == CellToLocationDict.TryGetValue(cell, out Location ret))
        {
            ret = new Location(cell, currentLocation, endCell, this);
            CellToLocationDict.TryAdd(cell, ret);
        }

        return ret;

    }

    public List<Location> GetNeighbors(Location loc)
    {
        return loc.GetNeighboringLocations();
    }


}
