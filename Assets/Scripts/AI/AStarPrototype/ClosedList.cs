using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedList 
{

    private readonly List<Location> locList;

    public ClosedList() 
    { 
        locList = new List<Location>();
    
    }

    public void AddLocation(Location loc)
    {
        locList.Add(loc);
    }

    public bool Contains(Location loc)
    {
        if (locList.Count > 0) { return locList.Contains(loc); }
        else return false;

       
    }

    public void Remove(Location loc) {  locList.Remove(loc); }

}
