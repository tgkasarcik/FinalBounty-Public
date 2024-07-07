using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResolutionFinder
{
    private double radius;
    private readonly int defaultres = 13;
    private readonly SortedList<(double, double), int> RadiusRes = new SortedList<(double, double), int>()
    {
        { ( 0d, 7d ), 5 },
        { ( 7d, 10d ), 7 },
        { ( 20d, 24d ), 15 },
        { ( 24d, 27d ), 17 },
        { ( 27d, 30d ), 19 }
    };
    public ResolutionFinder(double radius)
    {
        this.radius = radius;
    }
    public int getResolution()
    {
        return BinarySearch(radius);
    }
    private bool InRange(double value, double min, double max)
    {
        if (value >= min && value < max)
        {
            return true;
        }
        return false;
    }
    //https://www.geeksforgeeks.org/binary-search/
    private int BinarySearch(double target)
    {
        int low = 0, high = RadiusRes.Count - 1;
        
        while (low <= high)
        {
            int mid =  low + (high-low) / 2;
            //current index
            //if in range return the value
            Debug.Log("index:" +mid.ToString());
            if (InRange(target, RadiusRes.Keys[mid].Item1, RadiusRes.Keys[mid].Item2))
            {
                return RadiusRes.Values[mid];
            }
            //if target is larger than range then ignore left half of dictionary
            if (RadiusRes.Keys[mid].Item1 < target)
            {
                low = mid + 1;
            }
            //if target is smaller than range then ignore right half of dictionary
            else
            {
                high = mid - 1;
            }
            

        }
        return defaultres;
    }
}
