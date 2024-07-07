using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Rendering;

namespace PlanetGen
{
    public class Spawner
    {
        //This dictionary contains gameobjects and their spawn likelihoods
        private SortedList<(float, float), GameObject> RangeDict;
        private readonly GridSystem Grid;
        private static System.Random rnd;
        private readonly GameObject parent;
        private GameObject player;
        private Vector3 scale;
        private GenPlanet planet;
        public Spawner(GridSystem gridSystem, Dictionary<GameObject, float> GOPercent, int seed, GameObject parent, GameObject player, GenPlanet planet) {
            this.Grid = gridSystem;
            SetRangeDict(GOPercent);
            this.parent = parent;
            rnd = new System.Random(seed);
            this.planet = planet;
        }

        public void Spawn()
        {               
            Cell[] cells = Grid.GetCells();
            cells[0].SetFilled(true);
            foreach (Cell cell in cells)
            {
                var cmp = (float)rnd.NextDouble();
                var prefab = BinarySearch(cmp);

                if (prefab != null && !cell.IsFilled())
                {
                    //Debug.Log(prefab.ToString());
                    LoadInPrefab(cell, prefab);
                    cell.SetFilled(true);
                }
            }
        }

        public GameObject SpawnPrefabAtOrigin(GameObject prefab)
        {
            return LoadInPrefab(Grid.GetCells()[0], prefab);
        }

        public GameObject SpawnPrefabAtRandom(GameObject prefab)
        {
            int index = rnd.Next(1, Grid.GetCells().Length);
            return LoadInPrefab(Grid.GetCells()[index], prefab);
        }

        public void PutObjectAtOrigin(GameObject tempObj)
        {
            tempObj.transform.position = Grid.GetCells()[0].center;
            tempObj.transform.parent = parent.transform;
            Vector3 grav = (tempObj.transform.position - parent.transform.position).normalized;
            tempObj.transform.rotation = Quaternion.FromToRotation(tempObj.transform.up, grav) * tempObj.transform.rotation;
        }

        private GameObject LoadInPrefab(Cell cell, GameObject prefab)
        {

            //Debug.Log("Prefab tags: " + prefab.tag + " name is " + prefab.name);

            //For whatever reason, we cannot access the tags here?
            //So instead, we hard code a bunch of names...
            //If I had time I would find a better solution 
            if (prefab.name == "Drone" || prefab.name == "Ram" || prefab.name == "SmallBoi" || prefab.name == "MissileShip")
            {

                SpawnEnemy temp = new SpawnEnemy(cell, prefab, this.planet);
                return temp.GetInstantiatedPrefab();


            }
            else
            {
                var obj = Transform.Instantiate(prefab, cell.center, Quaternion.identity);
                obj.transform.parent = parent.transform;
                Vector3 grav = (obj.transform.position - parent.transform.position).normalized;
                obj.transform.rotation = Quaternion.FromToRotation(obj.transform.up, grav) * obj.transform.rotation;
                obj.SetActive(true);
                return obj;
            }
        }
        //The range is set for the low end to be inclusive and the high end to be exclusive
        private void SetRangeDict(Dictionary<GameObject, float> GOPercent)
        {
            float current = 0;
            this.RangeDict = new SortedList<(float, float), GameObject>();
            foreach (var (obj, percent) in GOPercent)
            {
                if (percent > 0)
                {
                    this.RangeDict.Add(new(current, percent + current), obj);
                    
                    current += percent;
                }
                
            }
            if (current < 1f)
            {
                this.RangeDict.Add(new(current, 1), null);
            }
        }
        private bool InRange(float value, float min, float max)
        {
            if (value>=min && value < max)
            {
                return true;
            }
            return false;
        }
        //https://www.geeksforgeeks.org/binary-search/
        private GameObject BinarySearch(float target)
        {
            int low = 0, high = RangeDict.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                //current index
                //if in range return the value
                Debug.Log("index:" + mid.ToString());
                if (InRange(target, RangeDict.Keys[mid].Item1, RangeDict.Keys[mid].Item2))
                {
                    return RangeDict.Values[mid];
                }
                //if target is larger than range then ignore left half of dictionary
                if (RangeDict.Keys[mid].Item1 < target)
                {
                    low = mid + 1;
                }
                //if target is smaller than range then ignore right half of dictionary
                else
                {
                    high = mid - 1;
                }


            }
            return null;
        }

    }
}


