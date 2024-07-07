using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//Code from: https://github.com/joeythelantern/AsteroidBeltExample

public class BeltSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<GameObject> asteroidPrefabs;
    public int cubeDensity = 100;
    public int seed;
    public float innerRadius;
    public float outerRadius;
    public float height = 0f;
    public bool rotatingClockwise = false;

    [Header("Asteroid Settings")]
    public float minOrbitSpeed = 10;
    public float maxOrbitSpeed = 15f;
    public float minRotationSpeed = 1f;
    public float maxRotationSpeed = 5f;

    private GameObject asteroidPrefab;
    private Vector3 localPosition;
    private Vector3 worldOffset;
    private Vector3 worldPosition;
    private float randomRadius;
    private float randomRadian;
    private float x;
    private float y;
    private float z;

    //================================================
    // Random Point on a Circle given only the Angle.
    // x = cx + r * cos(a)
    // y = cy + r* sin(a)
    //================================================
    private void Start()
    {
        Random.InitState(seed);

        for (int i = 0; i < cubeDensity; i++)
        {
            asteroidPrefab = GenerateRandomPrefab(asteroidPrefabs);
            do
            {
                randomRadius = Random.Range(innerRadius, outerRadius);
                randomRadian = Random.Range(0, (2 * Mathf.PI));

                y = Random.Range(-(height / 2), (height / 2));
                x = randomRadius * Mathf.Cos(randomRadian);
                z = randomRadius * Mathf.Sin(randomRadian);
            }
            while (float.IsNaN(z) && float.IsNaN(x));

            localPosition = new Vector3(x, y, z);
            worldOffset = transform.rotation * localPosition;
            worldPosition = transform.position + worldOffset;

            GameObject _asteroid = Instantiate(asteroidPrefab, worldPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            _asteroid.AddComponent<BeltObject>().SetupBeltObject(Random.Range(minOrbitSpeed, maxOrbitSpeed), Random.Range(minRotationSpeed, maxRotationSpeed), gameObject, rotatingClockwise);

            if (_asteroid.GetComponent<Rigidbody>() == null)
            {
                var rb = _asteroid.AddComponent<Rigidbody>();
                rb.useGravity = false;
            }

            //Makes sure the mesh collider is convex to not piss off unity
            MeshCollider meshCollider = _asteroid.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                var collider = _asteroid.AddComponent<MeshCollider>();
                collider.convex = true;
            }
            else
            {
                meshCollider.convex = true;
            }
            

            _asteroid.transform.SetParent(transform);
        }

       SpawnParimeter();
    }

    //Makes the parimeter so the player cannot leave the solar system
    private void SpawnParimeter()
    {
        //Incriments how close together the boxes will be
        float incriment = 0.1f;

        GameObject bounds = new GameObject();
        bounds.name = "Asteroid Belt";

        //Creates a lot of boxs in a circle
        for(float i = 0; i < 2 * Mathf.PI; i += incriment)
        {
            GameObject colliderContainer = new GameObject();
            colliderContainer.layer = LayerMask.NameToLayer("Asteroids");
            colliderContainer.name = "BounderBox";
            Transform boxTransform = colliderContainer.GetComponent<Transform>();
            BoxCollider box = colliderContainer.AddComponent<BoxCollider>();
            box.size = new Vector3(innerRadius, 1f, innerRadius / 4);
            boxTransform.position = transform.position + (innerRadius *  new Vector3(Mathf.Cos(i), 0f, Mathf.Sin(i)));
            Quaternion rot = Quaternion.LookRotation(boxTransform.position - transform.position, Vector3.up);
            boxTransform.rotation = rot;
            boxTransform.parent = bounds.transform;
        }
    }

    private GameObject GenerateRandomPrefab(List<GameObject> list)
    {
        int index = Random.Range(0, list.Count);
        GameObject prefab = list[index];
        return prefab;
    }
}