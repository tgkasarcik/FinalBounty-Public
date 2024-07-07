using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using JSON;
using System.Threading;

//wrapper class to add more functionality and manage the Level Graph
public static class PlayerLevelManager{

    public static LevelGraph levelGraph;
    private static Sprite circleSprite;
    private static Sprite currentLevelSprite;
    public static int playerPosition;
    public static SceneLoader loader;
    public static string levelScene;
    public static int graphSeed;
    public static int currentLevelSeed = -1;
    public static GameObject mapCanvas;
    private static RectTransform graphContainer;
    private static UIGraph graphUI;
    public static int minPlanets;
    public static int maxPlanets;
    private static int currentLevel;

    public static List<GameObject> PlanetPrefabs;
    public static List<GameObject> SunPrefabs;
    public static List<GameObject> BossPrefabs;
    public static List<GameObject> AsteroidPrefabs;

    private static int[] nodeToSeedMap;


    //creates new level graph and updates variables
    public static void Initialize(SceneLoader loaderTemp, string levelSceneTemp, Sprite circleSpriteTemp, Sprite currentLevelSpriteTemp, GameObject mapCanvasTemp, int minPlanetsTemp, int maxPlanetsTemp, List<GameObject> PlanetPrefabsTemp, List<GameObject> SunPrefabsTemp, List<GameObject> BossPrefabsTemp, List<GameObject> AsteroidPrefabsTemp)
    {
        
        loader = loaderTemp;
        levelScene = levelSceneTemp;
        circleSprite = circleSpriteTemp;
        currentLevelSprite = currentLevelSpriteTemp;
        mapCanvas = mapCanvasTemp;
        graphContainer = mapCanvas.transform.Find("Map_Graph").Find("graphContainer").GetComponent<RectTransform>();
        UnityEngine.Object.DontDestroyOnLoad(mapCanvas);
        
        minPlanets = minPlanetsTemp;
        maxPlanets = maxPlanetsTemp;
        PlanetPrefabs = PlanetPrefabsTemp;
        SunPrefabs = SunPrefabsTemp;
        BossPrefabs = BossPrefabsTemp;
        AsteroidPrefabs = AsteroidPrefabsTemp;
    }
    public static int GetCurrentLevel()
    {
        return currentLevel;
    }
    //creates a new bidirenctional cyclic graph with parameters specified
    public static void createGraph(int numLevels, int numEdges, int displayRows = 2, float xOffset = 20f, float yOffset = 20f, int seedTemp = -1)
    {
        //create graph
        if(seedTemp == -1)
        {
            graphSeed = Guid.NewGuid().GetHashCode();
        }
        else{
            graphSeed = seedTemp;
        }
		levelGraph = new LevelGraph(levelScene, numLevels, numEdges);
		levelGraph.generateLevelGraph(seedTemp);

        //create seeds for planets
        nodeToSeedMap = new int[numLevels];
        generateLevelSeeds();
        graphUI = new UIGraph(numLevels, graphContainer, circleSprite, currentLevelSprite, levelGraph.graph);
        graphUI.createUIGraph(displayRows, xOffset, yOffset);
    }

    //creates seed numbers for all planets
    private static void generateLevelSeeds()
    {
        for(int i = 0; i < levelGraph.graph.Length; i++)
        {
            nodeToSeedMap[i] = Guid.NewGuid().GetHashCode();
            
        }
        
    }

    public static int GetCurrentLevelSeed()
    {
        return currentLevelSeed;
    }

    //loads in first level of level (starting position)
    public static void start()
    {
        currentLevel = 0;
        loadInLevel(0);
        
    }

    private static void loadInLevel(int level)
    {
        playerPosition = level;
        currentLevelSeed = nodeToSeedMap[level];
        PlanetRoomManager.Initialize(currentLevelSeed);
        //Added by Tala
        //This is data from JSON file in Resources/Text/MapPrefabs.cs
        LevelPrefabData[] data = loadInPrefabData()[level];
        //set planet amount here
        PlanetRoomManager.SetPlanetAmount(minPlanets, maxPlanets);
        //In planetroommanager, the prefabs will be randomly selected based on the number of planets
        PlanetRoomManager.SetEnemyPrefabs(data);
        PlanetRoomManager.SetPlanetPrefabs(PlanetPrefabs, SunPrefabs, BossPrefabs, AsteroidPrefabs);
        graphUI.setCurrentLevel(level);
        PlanetRoomManager.CreateSolarSystem();
        loader.StartLoad(levelGraph.sceneToUse);
        
    }
    private static LevelPrefabData[][] loadInPrefabData()
    {
        JSONReader reader = new JSONReader();
        LevelPrefabData[][] data = reader.getData();
        return data;
    }
    public static void move(int level)
    {
        //checks if movment is valid and then moves to specified level
        if(levelGraph.graph[playerPosition].Contains(level))
        {
            loadInLevel(level);
        }
    }

    //selects a node connecting to the current node at random and moves there
    public static void moveRandom()
    {
        if(levelGraph.graph[playerPosition].Count > 0)
        {
            int levelIndex = (int)UnityEngine.Random.Range(0, levelGraph.graph[playerPosition].Count);
            int nextLevel = levelGraph.graph[playerPosition][levelIndex];
            currentLevel = nextLevel;
            loadInLevel(nextLevel);
        }
    }
   
    public static void MoveNext()
    {
        var count = levelGraph.graph[playerPosition].Count;
        
        if (count > 0)
        {
            //hardcoded
            if (currentLevel < 2)
            {
                
                loadInLevel(currentLevel + 1);
                currentLevel++;
            }
            else
            {
                loader.StartLoad("Win Scene");
                //Where we would load in the final scene
            }
            Debug.Log(currentLevel);

        }
    }
    public static void toggleUIGraph()
    {
        mapCanvas.SetActive(!mapCanvas.activeSelf);
    }

    public static void outputCurrentLevelStats()
    {
        Debug.Log("Current Level: " + playerPosition);

        //gets the nodes connected to the level node
        string connections = "";
        if(levelGraph.graph[playerPosition].Count > 0)
        {
            for(int i = 0; i < levelGraph.graph[playerPosition].Count - 1; i++)
            {
                connections += levelGraph.graph[playerPosition][i] + ", ";
            }
            connections += levelGraph.graph[playerPosition][levelGraph.graph[playerPosition].Count - 1];
        }
        else
        {
            connections += "None";
        }

        Debug.Log("Current Level is Connected to: " + connections);
    }
}
