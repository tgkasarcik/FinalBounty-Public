using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetGen;

public class GenerateEnemies : MonoBehaviour
{
    [SerializeField]
    public GameObject Planet;

    [SerializeField]
    public int Resolution;

    [SerializeField]
    public GameObject Player;

    [SerializeField] public float planetScale;
    [SerializeField] public List<GameObject> Enemies;
    [SerializeField] public List<float> EnemySpawnPercent;
    [SerializeField] private int planetSeed;

    void Awake()
    {
        GenPlanet spawnEnemies = new GenPlanet(Planet, Enemies, EnemySpawnPercent, 0, 0, 0, 0, 1, 9.8, planetSeed);
        spawnEnemies.Planet = Planet;
        spawnEnemies.resolution = Resolution;
        spawnEnemies.LoadSpawns();
        spawnEnemies.SpawnThings();
        spawnEnemies.LoadInPlayer(Player);
    }
}
