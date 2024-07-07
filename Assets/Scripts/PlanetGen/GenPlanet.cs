using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using PlanetGen;
using UnityEngine.UIElements;
using Upgrades;

namespace PlanetGen
{
    
    public class GenPlanet
    {
        
        public int resolution;
        public double mass;
        public bool spawnsInit;
        public double radius;
        public double gravity;
        //orbit speed is in radians per second
        public double orbitSpeed;
        //rotation speed is in radians per second
        public double rotationSpeed;
        public GameObject Planet;
        public IUpgrade upgrade;
        public GameObject upgradeObj;
        public GameObject upgradePrefab;

        public bool isShop;
        public bool isBoss;

        public bool isCleared;


        public GameObject Player;

        //global is how many meters in global space
        public float planetScaleGlobal;
        //local is scaling according to the prefab
        public float planetScaleLocal;


        private GameObject Prefab;
        public List<GameObject> Enemy;
        public List<float> EnemySpawnPercent;
        public Planet planetBall;
        private Dictionary<GameObject, float> PrefabDictionary;
        public GridSystem Grid;
        public Spawner Spawn;
        private int planetSeed;

        public GenPlanet(GameObject prefab, List<GameObject> Enemies, List<float> EnemySpawnPercent, double mass,
        double rotSpeed, double orbitSpeed, double radius, float scale, double gravity, int seed, bool isShop = false, bool isBoss = false)
        {
            this.upgradePrefab = (GameObject)Resources.Load("Prefabs/Upgrades/Upgrade");
            //init private variables
            this.Prefab = prefab;
            this.gravity = gravity;
            this.radius = radius;
            this.Enemy = Enemies;
            this.rotationSpeed = rotSpeed;
            this.orbitSpeed = orbitSpeed;
            this.mass = mass;
            this.EnemySpawnPercent = EnemySpawnPercent;
            this.planetSeed = seed;
            this.planetScaleGlobal = scale;
            this.isShop = isShop;
            this.isBoss = isBoss;
            this.isCleared = false;
            this.spawnsInit = false;
            SetResolution();
            var temp = prefab.GetComponent<MeshRenderer>().bounds.size.x;
            float newScale = planetScaleGlobal / temp;
            this.planetScaleLocal = newScale * prefab.transform.localScale.x;

            this.upgrade = UpgradeManager.GetRandomPlanetPoolUpgrade(new System.Random(seed));
            
            LoadPrefabDictionary();
        }
        //This creates vertices and spawns the prefabs on them based on percentages
        private void SetResolution()
        {
            ResolutionFinder resolutionFinder = new ResolutionFinder(radius);
            resolution = resolutionFinder.getResolution();
            //Debug.Log("isshop/isboss: " + (isShop || isBoss ).ToString() +" resolution: "+(resolution).ToString()+ "radius: " + (radius).ToString());
        }
        public void LoadPrefabDictionary()
        {
            PrefabDictionary = new Dictionary<GameObject, float>();
            for (int i = 0; i < this.Enemy.Count; i++)
            {
                
                PrefabDictionary.Add(this.Enemy[i], this.EnemySpawnPercent[i]);
            }
        }
        public void LoadSpawns()
        {
            var planScale = new Vector3(planetScaleGlobal, planetScaleGlobal, planetScaleGlobal);
            
            planetBall = new Planet(Planet, resolution, planScale);
            planetBall.GenerateMesh();

            Grid = new GridSystem(resolution, planetBall.getRectangles(), planetBall.getVertices(), Planet, planScale);
            Grid.GenerateCells();

            Spawn = new Spawner(Grid, PrefabDictionary, this.planetSeed, Planet, Player, this);
            this.spawnsInit = true;
        }

        public void SpawnThings()
        {
            if(!this.isCleared)
            {
                this.Spawn.Spawn();
            }
            else
            {
                SpawnUpgrade();
            }
        }

        public GameObject LoadInPlayer(GameObject player)
        {
            return this.Spawn.SpawnPrefabAtOrigin(player);
        }

        public void PutPlayerOnPlanet(GameObject player)
        {
            this.Spawn.SpawnPrefabAtOrigin(player);
        }

        //makes spawning enemies cease and spawns in pickup
        public void SetCleared()
        {
            this.isCleared = true;
            SpawnUpgrade();
            //gets notifications text in HUD and 
            HUDNotifications.MakeNotification("Upgrade Spawned");
        }

        public void SpawnUpgrade()
        {
            this.upgradeObj = (GameObject)this.Spawn.SpawnPrefabAtRandom(this.upgradePrefab);
            var handler = this.upgradeObj.GetComponent<TouchUpgradeHandler>();
            handler.upgrade = this.upgrade;
            var gravBod = this.upgradeObj.AddComponent<GravityBody>();
            gravBod.SetPlanetObj(this.Planet);
            WaypointManager.MakeWaypoint(this.upgradeObj, "item", upgrade.name);
        }

        //generates the gameobject for planet
        public GameObject CreatePlanetGameObject(Vector3 position, bool isSun = false, bool isBoss = false)
        {

            //create planet game object
            
            GameObject planetSphere = GameObject.Instantiate(Prefab, position, Quaternion.identity);
            //adjusts planet gameobject size to appropriate scale;
            planetSphere.transform.localScale = new Vector3(planetScaleLocal,planetScaleLocal,planetScaleLocal);
            planetSphere.transform.position = position;
            if(isSun)
            {
                planetSphere.tag = "Sun";
                ParticleSystem pSys = planetSphere.GetComponentInChildren<ParticleSystem>();
                if(pSys != null)
                {
                    pSys.transform.localScale = new Vector3(planetScaleLocal, planetScaleLocal, planetScaleLocal);
                }
            }
            else if (isBoss)
            {
                planetSphere.tag = "Boss";
            }
            else
            {
                planetSphere.tag = "Planet";
            }


            this.Planet = planetSphere;

            return planetSphere;
        }

       
    }
}