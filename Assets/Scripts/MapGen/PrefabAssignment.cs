using JSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabAssignment
{
    private List<GameObject>[] prefabs;
    private List<float>[] percents;
    private int numPlanets;
    private int dataLength;
    private LevelPrefabData[] levelData;
    private static System.Random rnd;
    public PrefabAssignment(LevelPrefabData[] levelPrefabData, int numPlanets, int seed)
    {

        levelData = levelPrefabData;
        this.numPlanets = numPlanets;
        rnd = new System.Random(seed);
        dataLength = levelData.Length;
        prefabs = new List<GameObject>[numPlanets];
        percents = new List<float>[numPlanets];
        defineAssignments();
    }
    //https://stackoverflow.com/questions/273313/randomize-a-listt
    private void defineAssignments()
    {
        for (int i = 0; i < numPlanets; i++)
        {
            int random = rnd.Next(dataLength);
            prefabs[i] = LoadPrefabs(levelData[random].prefabs);
            percents[i] = levelData[random].percents.ToList();

        }
    }
    private List<GameObject> LoadPrefabs(string[] filePaths)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (string path in filePaths)
        {
            GameObject GO = Resources.Load<GameObject>(path);
            list.Add(GO);
        }
        return list;

    }
    public List<GameObject>[] GetPrefabs()
    {
        return prefabs;
    }

    public List<float>[] getPercents()
    {
        return percents;
    }
}
