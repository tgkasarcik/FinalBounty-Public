using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=fsDE_mO4RZM
public class ObjectPool 
{
    private PoolableObject Prefab;
    private List<PoolableObject> AvailableObjects;

    private ObjectPool(PoolableObject prefab, int size)
    {
        this.Prefab = prefab;
        AvailableObjects = new List<PoolableObject>();
    }

    public static ObjectPool CreateInstance(PoolableObject prefab, int size)
    {
        ObjectPool pool = new ObjectPool(prefab, size);

        GameObject poolObject = new GameObject(prefab.name + " Pool");
        pool.CreateObjects(poolObject.transform, size);

        return pool;

    } 

    private void CreateObjects(Transform parent, int size)
    {
        for (int i = 0; i < size; i++) 
        { 
            PoolableObject poolableObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false);
        }
    }

    public void ReturnObjectToPool(PoolableObject poolableObject)
    {
        AvailableObjects.Add(poolableObject);
    }

    public PoolableObject GetObject()
    {
        if (AvailableObjects.Count > 0)
        {
            PoolableObject instance = AvailableObjects[0];
            AvailableObjects.RemoveAt(0);

            instance.gameObject.SetActive(true);

            return instance;
        } 
        else
        {
            //could also just make another object instead of returning null
            Debug.LogError($"Could not get an object from pool \"{Prefab.name}\" Pool. Probably a config issue");
            return null;
        }
    }



}
