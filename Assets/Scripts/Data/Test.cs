using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSON;
using static JSON.JSONReader;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSONReader reader = new JSONReader();
        LevelPrefabData[][] data = reader.getData();
        

        for (int i = 0; i < data.Length; i++)
        {

            var current = data[i];  
            
            for (int j = 0; j < current.Length; j++)
            {
                var curr = current[j];

                    foreach (string prefab in curr.prefabs)
                    {
                        Debug.Log(prefab);
                    }
                    foreach (float percent in curr.percents)
                    {
                        Debug.Log(percent);
                    }
                
            }
        }
    }
}
