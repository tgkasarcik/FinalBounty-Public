using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetGen;
using System;
using JSON;
using PlayerObject;
public static class PlanetRoomManager
{
    private static int levelSeed;
    public static bool didInit = false;
    public static bool isPopulated = false;
    public static SolarSystem solarSystem;
    public static List<GameObject> Enemies;
    public static List<float> spawnPercents;
    private static SceneLoader loader;
    private static LevelPrefabData[] prefabData;
    private static int ClearedPlanetsNum;
    private static int PlanetNum;
    //amount a planet gets scaled by in a planet room scene
    public static int planetUpscale = 4;
    public static GenPlanet currentPlanet;
    //keeps track of Level and Planet progression
    private static ProgressManager Progress;
    public static void Initialize(int seed = -1)
    {
        if(seed == -1)
        {
            levelSeed = Guid.NewGuid().GetHashCode();
        }
        else
        {
            levelSeed = seed;
        }
        didInit = true;

        solarSystem = new SolarSystem(seed);

        GameObject temp = GameObject.Find("SceneLoader");
        solarSystem.GetNumPlanets();
        //if there is no scene loader create a new empty sceneloader
        if(temp == null)
        {
            GameObject tempObj = new GameObject("SceneLoader");
            loader = tempObj.AddComponent<SceneLoader>();
        }
        else
        {
            loader = temp.GetComponent<SceneLoader>();
        }
        Progress = ProgressManager.GetInstance();
        //removed setEnemyPrefabs call, is now called by PlayerLevelManager
    }
    //finds how many planets are in the solar systeme
    // must be called before CreateSolarSystem
    public static void SetPlanetAmount(int minPlanets, int maxPlanets)
    {
        solarSystem.SetPlanetAmount(minPlanets, maxPlanets);
    }

    //creates a new solar system and returns the list of GenPlanets

    public static List<GenPlanet> CreateSolarSystem()
    {
        List<GenPlanet> planets = solarSystem.CreateSolarSystem();
        isPopulated = true;
        PlanetNum = planets.Count;
        return planets;
    }

    //creates a new solar system and returns the list of GenPlanets
    public static void CreateSolarSystemObjects()
    {
        solarSystem.CreateSolarSystemObjects();
    }
    //changed by Tala

    //sets the enemy prefab lists

    public static void SetEnemyPrefabs(LevelPrefabData[] levelPrefabData)
    {
        int numPlanets = solarSystem.GetNumPlanets();
        PrefabAssignment assignment = new PrefabAssignment(levelPrefabData, numPlanets, levelSeed);
        List<GameObject>[] enemySpawnObjects = assignment.GetPrefabs();
        List<float>[] enemySpawnPercentages = assignment.getPercents();
        //call enemy selection class
        solarSystem.SetEnemySpawns(enemySpawnObjects, enemySpawnPercentages);
    }
    /*
    private static void SetDefaultEnemies()
    {
        Enemies = new List<GameObject>();
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.GetComponent<Renderer>().material.color = Color.red;
        Enemies.Add(enemy);

        spawnPercents = new List<float>();
        spawnPercents.Add(0.25f);
    }
    */
    //sets the planet prefab lists
    public static void SetPlanetPrefabs(List<GameObject> PlanetPrefabs, List<GameObject> SunPrefabs, List<GameObject> BossPrefabs, List<GameObject> AsteroidPrefabs)
    {
        solarSystem.SetPlanetPrefabs(PlanetPrefabs, SunPrefabs, BossPrefabs, AsteroidPrefabs);
    }

    //returns back to solar system view of planets
    public static void ExitPlanet()
    {
        //update external JSON file with what happened

        //load in solar system scene
        currentPlanet.planetScaleLocal  = currentPlanet.planetScaleLocal / planetUpscale;
        currentPlanet.planetScaleGlobal = currentPlanet.planetScaleGlobal / planetUpscale;

        foreach(IPlayer player in PlayerManager.players)
        {
            player.onPlanet = false;
        }
        
        loader.StartLoad("SolarSystem");
        //Removes collider, edit the prefab of the planet to be red
        //TODO add info that makes it so player cannot enter.
    

        //May be troublesome
        currentPlanet = null;

    }
    
    //enters in a gameobject planet specified
    public static void EnterPlanet(GameObject planet)
    {

        GenPlanet tempPlanet = FindGenPlanet(planet);
        if(tempPlanet == null)
        {
            Debug.Log("Could not find planet referenced!");
        }
        else{
            currentPlanet = tempPlanet;
            EnemyCounter.SetPlanet(tempPlanet);
            foreach(IPlayer player in PlayerManager.players)
            {
                player.onPlanet = true;
            }
            loader.StartLoad("PlanetRoom");
        }
    }

    public static GameObject CreateCurrentPlanet(Vector3 position)
    {
        GameObject planet = currentPlanet.CreatePlanetGameObject(position, false, currentPlanet.isBoss);
        SolarSystemPlanet gravity = currentPlanet.Planet.AddComponent<SolarSystemPlanet>();
        gravity.Initialize(currentPlanet.mass, currentPlanet.radius, new Vector3(0,0,0), 0, 0, currentPlanet.gravity);
        return planet;
    }


    //finds a GenPlanet object from a given planet gameobject
    public static GenPlanet FindGenPlanet(GameObject planet)
    {
        List<GenPlanet> planets = solarSystem.planets;
        for(int i = 0; i < planets.Count; i++)
        {
            if(planet == planets[i].Planet)
            {
                return planets[i];
            }
        }
        return null;
    }

    public static Boolean IsLevelCleared()
    {
        return Progress.IsLevelCleared(PlayerLevelManager.GetCurrentLevel());
    }
}
