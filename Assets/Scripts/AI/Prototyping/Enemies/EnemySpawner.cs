using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=5uO0dXYbL-s&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=46
public class EnemySpawner : MonoBehaviour
{
    public Transform Player;
    public int NumberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<Enemy> EnemyPrefabs = new List<Enemy>();
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();


    private void Awake()
    {
        for (int i = 0; i < EnemyPrefabs.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], NumberOfEnemiesToSpawn));
        }
    }

    private void Start()
    {
        Triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        int SpawnedEnemies = 0;

        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if (EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(SpawnedEnemies);
            }
            else if (EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            SpawnedEnemies++;

            yield return Wait;
        }
    }

    private void SpawnRoundRobinEnemy(int SpawnedEnemies)
    {
        int SpawnIndex = SpawnedEnemies % EnemyPrefabs.Count; //Lets us spawn one of each enemy, then do it again

        DoSpawnEnemy(SpawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, EnemyPrefabs.Count));
    }

    private void DoSpawnEnemy(int SpawnIndex)
    {
        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();

            //Lets us spawn an enemy on any vertex within a navmesh

            //Pick a random vertex on the navmesh
            int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out NavMeshHit Hit, 2f, 1)) //1 for walkable areas
            {
                //Teleport to the vertex
                enemy.Agent.Warp(Hit.position);

                //Need to enable enemy to get it to start chasing
                enemy.Movement.Player = Player;
                enemy.Agent.enabled = true;
                enemy.Movement.Triangulation = Triangulation;

                enemy.Movement.StartMovement();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh {Triangulation.vertices[VertexIndex]}");
            }


        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type ${SpawnIndex} from object pool. Are we out of objects?");
        }
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }


}
