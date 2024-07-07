using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class OpenList 
{

    private readonly List<Location> LocationList;
    private readonly List<Vector3> StoredVectors;


    public OpenList()
    {
        this.LocationList = new List<Location>();
        this.StoredVectors = new List<Vector3>();
    }

    public void AddLocation(Location loc)
    {
        //if (false == checkIfVectorExists(loc)) {
        //    this.LocationList.Add(loc);
        //    Sort();
        //    StoredVectors.Add(loc.MyCell.getLocation());
        //} else
        //{
        //    Debug.Log("No way this works :!!");
        //}

        this.LocationList.Add(loc);
        Sort();

    }

    public void Sort()
    {
        //Sort by score
        if (this.LocationList.Count > 1)
        {
            LocationList.Sort(delegate (Location a, Location b)
            {
                //a and b should never be equivalent... Mathf.sign will never return 0... this shouldn't be an issue? 
                return (int)Mathf.Sign(a.Score - b.Score);
            });
        }
    }

    public Location GetBestLocation()
    {
        Location ret = LocationList.First();
        LocationList.RemoveAt(0);
        return ret;
    }

    public bool Contains(Location loc)
    {
        if (LocationList.Count > 0) { return LocationList.Contains(loc); }
        else return false;

       
    }

    //Inefficient
    private bool checkIfVectorExists(Location loc)
    {
        Vector3 myVector = loc.MyCell.getLocation();

        foreach(Vector3 v in StoredVectors) 
        { 
            if (v.Equals(myVector)) return true;
        }

        return false;
    }



    //DEBUG METHODS
    public void PrintList()
    {

        foreach (Location loc in this.LocationList)
        {
            Debug.Log("Location: " + loc.ToString() + " Score: " + loc.Score);
        }
    }

    public string getLen()
    {
        return LocationList.Count.ToString();
    }




}
