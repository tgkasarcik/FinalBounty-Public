using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetGen;
public class GenerateAIEnemies : MonoBehaviour
{
    [SerializeField]
    public GameObject Planet;

    [SerializeField]
    public int Resolution;

    //[SerializeField]
    //public GameObject Player;

    [SerializeField] public float planetScale;
    [SerializeField] public List<GameObject> Enemies;
    [SerializeField] public List<float> EnemySpawnPercent;
    [SerializeField] private int planetSeed;
    [SerializeField] public GameObject DebugEnemy;

    public GenPlanet myPlanet;

    void Awake()
    {
        GenPlanet spawnEnemies = new GenPlanet(Planet, Enemies, EnemySpawnPercent, 0, 0, 0, 0, planetScale, 9.8, planetSeed);
        spawnEnemies.Planet = Planet;
        spawnEnemies.resolution = Resolution;
        spawnEnemies.LoadSpawns();
        spawnEnemies.SpawnThings();
        GameObject planetScript = GameObject.Find("PlanetScript");
        Tester visual = planetScript.GetComponent<Tester>();
        visual.MyPlanet = spawnEnemies;
        visual.DebugEnemy = this.DebugEnemy;
    }


    

}
