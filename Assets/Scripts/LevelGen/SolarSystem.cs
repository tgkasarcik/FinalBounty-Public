using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using PlanetGen;
using Unity.VisualScripting;

public class SolarSystem
{
    private int levelSeed;

    public List<GenPlanet> planets;
    public List<Vector3> planetPositions;
    public List<int> planetSeeds;
    //these are now arrays so that each planet has different enemy spawns
    public List<GameObject>[] enemySpawnObjects;
    public List<float>[] enemySpawnPercentages;

    public List<GameObject> PlanetPrefabs;
    public List<GameObject> PlanetPrefabsCopy;
    public List<GameObject> SunPrefabs;
    public List<GameObject> BossPrefabs;
    public List<GameObject> AsteroidPrefabs;
    //Boss Level
    private int bossIndex;
    
    //shop ship params
    private GameObject shopShipPrefab;
    private float shopShipSize = 5f;
    public GameObject shopShip;
    public int shopShipIndex;

    //planet params
    private double maxPlanetSize = 20.0;
    private double minPlanetSize = 6.0;
    private double maxPlanetMass = 180.0;
    private double minPlanetMass = 100.0;
    private double maxPlanetRotationSpeed = 0.0001;
    private double minPlanetRotationSpeed = 0.000001;

    //sun params
    private double maxSunSize = 60.0;
    private double minSunSize = 40.0;
    private double maxSunMass = 700.0;
    private double minSunMass = 200.0;
    private double maxSunRotationSpeed = 0.0001;
    private double minSunRotationSpeed = 0.000001;

    //solar system params
    //distance is calculated from each planet's edge not the center (to prevent overlap)
    private double maxNextPlanetDistance = 30;
    private double minNextPlanetDistance = 10;
    private int oribitSpeedModifier = 8000;
    private int planetRotationSpeedModifier = 12000;
    private int triggerBoxMetersAwayFromPlanet = 4;

    //generated params
    public double sunMass;
    public Vector3 sunPos;
    public int numPlanets;
    public System.Random levelRandom;

    private const double GRAVITATIONAL_CONSTANT = 0.00000000006673;

    //randomly generates number of planets from the seed given
    public SolarSystem(int seed = -1)
    {
        //generate new seed if necessary
        if(seed == -1)
        {
            this.levelSeed = Guid.NewGuid().GetHashCode();
        }
        else
        {
            this.levelSeed = seed;
        }

        //init fields
        levelRandom = new System.Random(this.levelSeed);
        this.planets = new List<GenPlanet>();
        this.planetPositions = new List<Vector3>();
        this.planetSeeds = new List<int>();

        this.shopShipPrefab = (GameObject)Resources.Load("Prefabs/Upgrades/Shop Ship");
        //This caused the warp bug
        //setDefaultPlanetSpawns();
    }


    //sets the spawn lists
    public void SetEnemySpawns(List<GameObject>[] enemySpawnObjects, List<float>[] enemySpawnPercentages)
    {
        this.enemySpawnObjects = enemySpawnObjects;
        this.enemySpawnPercentages = enemySpawnPercentages;
    }

    //sets the planet prefab lists
    public void SetPlanetPrefabs(List<GameObject> PlanetPrefabs, List<GameObject> SunPrefabs, List<GameObject> BossPrefabs, List<GameObject> AsteroidPrefabs)
    {
        this.PlanetPrefabs = PlanetPrefabs;
        this.PlanetPrefabsCopy = new List<GameObject>();
        this.SunPrefabs = SunPrefabs;
        this.BossPrefabs = BossPrefabs;
        this.AsteroidPrefabs = AsteroidPrefabs;
    }
    
    //creates a default enemy spawn and percentage
    /*
    private void setDefaultEnemySpawns()
    {
        this.enemySpawnObjects = new List<GameObject>();
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.GetComponent<Renderer>().material.color = Color.red;
        this.enemySpawnObjects.Add(enemy);
        enemy.hideFlags = HideFlags.HideInHierarchy;
        enemy.SetActive(false);

        this.enemySpawnPercentages = new List<float>();
        this.enemySpawnPercentages.Add(0.25f);
    }
    */

    //creates a default prefab for each planet type
    private void setDefaultPlanetSpawns()
    {

        this.PlanetPrefabs = new List<GameObject>();
        this.SunPrefabs = new List<GameObject>();

        GameObject planetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planetSphere.AddComponent<Gravity>();
        this.PlanetPrefabs.Add(planetSphere);
        //planetSphere.hideFlags = HideFlags.HideInHierarchy;
        //planetSphere.SetActive(false);

        GameObject sunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
        sunSphere.AddComponent<Gravity>();
        
        this.SunPrefabs.Add(sunSphere);
        //sunSphere.hideFlags = HideFlags.HideInHierarchy;
        //sunSphere.SetActive(false);


    }


    //creates a JSON File of all planets (maybe use planetRoom manager to do this)
    public void CreateSolarSystemFile()
    {
        //string text = File.ReadAllText(@"./person.json");
        //var person = JsonSerializer.Deserialize<Person>(text);
    }
    //called by planet room manager
    public void SetPlanetAmount(int minPlanets, int maxPlanets)
    {
        //satisfy preconditions
        if (minPlanets < 0)
        {
            minPlanets = 0;
        }
        if (maxPlanets < minPlanets)
        {
            maxPlanets = minPlanets + 1;
        }

        this.numPlanets = (int)levelRandom.Next(minPlanets, maxPlanets);
    }
    public List<GenPlanet> CreateSolarSystem(bool isShopIncluded = true, bool isBossIncluded = true)
    {
        this.shopShipIndex = -1;
        int numGenPlanets = numPlanets;
        if(isShopIncluded)
        {
            numGenPlanets = numPlanets + 1;
            shopShipIndex = levelRandom.Next(2, numPlanets);
        }

        if(isBossIncluded)
        {
            numGenPlanets = numPlanets + 1;
            int tempIndex;
            do
            {
                tempIndex = levelRandom.Next(1, numPlanets);
            } while (tempIndex == shopShipIndex);

            
            bossIndex = tempIndex;
        }

        GeneratePlanetSeeds(numGenPlanets);

        //creates sun at origin
        GenPlanet sun = CreateSun(planetSeeds[0], new Vector3(0,0,0), 0);
        planetPositions.Add(new Vector3(0,0,0));
        this.planets.Add(sun);

        Vector3 previousPlanetPos = new Vector3(0,0,0);


        //creates planets in a line (x axis) and adds them to the list
        for(int i = 1; i < numPlanets; i++)
        {
            if(i == shopShipIndex)
            {
                //generates shop ship where ever the index is located (wont call if not included in solar system)
                GenPlanet prevPlanet = this.planets[i - 1];
                double distanceBetween = ((this.maxNextPlanetDistance - this.minNextPlanetDistance) * levelRandom.NextDouble() + this.minNextPlanetDistance);

                Vector3 pos = previousPlanetPos + new Vector3((float)(prevPlanet.radius + distanceBetween + (this.shopShipSize / 2)),0,0);
                planetPositions.Add(pos);

                //adds planet to list
                GenPlanet tempShip = CreateShopShip(planetSeeds[i], pos, i);
                this.planets.Add(tempShip);
                previousPlanetPos = pos;
            }

            else
            {
                //generates next planets position and a random scale value
                GenPlanet prevObj = this.planets[this.planets.Count - 1];
                double distanceBetweenPlanets = ((this.maxNextPlanetDistance - this.minNextPlanetDistance) * levelRandom.NextDouble() + this.minNextPlanetDistance);

                float scale = (float)((this.maxPlanetSize - this.minPlanetSize) * levelRandom.NextDouble() + this.minPlanetSize);
                Vector3 planetPos = previousPlanetPos + new Vector3((float)(prevObj.radius + distanceBetweenPlanets + (scale / 2)), 0, 0);
                planetPositions.Add(planetPos);

                //Adds the boss level instead of a planet
                if (i == bossIndex)
                {
                    GenPlanet tempBoss = CreateBoss(planetSeeds[this.planets.Count], planetPos, i, scale);
                    this.planets.Add((tempBoss));
                    previousPlanetPos = planetPos;
                }
                else
                {
                    //adds planet to list
                    GenPlanet tempPlanet = CreatePlanet(planetSeeds[this.planets.Count], planetPos, i, scale);
                    this.planets.Add(tempPlanet);
                    previousPlanetPos = planetPos;
                }
            }
            
        }

        return this.planets;
    }
    public int GetNumPlanets()
    {
        return numPlanets;
    }
    public void GeneratePlanetSeeds(int numSeeds)
    {
        for(int i = 0; i < numSeeds; i++)
        {
            planetSeeds.Add(Guid.NewGuid().GetHashCode());
        }
    }

    public GenPlanet CreateBoss(int seed, Vector3 position, int index, float scale = -1.0f)
    {
        //create planet sphere with random scale and mass
        double mass = ((this.maxPlanetMass - this.minPlanetMass) * levelRandom.NextDouble() + this.minPlanetMass);

        //if no scale given then generate a random scale for planet
        if (scale == -1.0)
        {
            scale = (float)((this.maxPlanetSize - this.minPlanetSize) * levelRandom.NextDouble() + this.minPlanetSize);
        }

        //generate random rotation speed
        double planetRotSpeed = ((this.maxPlanetRotationSpeed - this.minPlanetRotationSpeed) * levelRandom.NextDouble() + this.minPlanetRotationSpeed);
        planetRotSpeed *= planetRotationSpeedModifier;

        //calculate oribital speed (https://www.physicsclassroom.com/class/circles/Lesson-4/Mathematics-of-Satellite-Motion#:~:text=As%20seen%20in%20the%20equation,the%20orbit%20affect%20orbital%20speed)
        double orbitSpeed = Math.Sqrt((GRAVITATIONAL_CONSTANT * sunMass) / Vector3.Distance(sunPos, position));
        orbitSpeed *= oribitSpeedModifier;

        GameObject prefab = GenerateRandomPrefab(BossPrefabs);

        //calc gavity https://emandpplabs.nscee.edu/cool/temporary/doors/space/planets/find/findFg.html
        double gravity = (GRAVITATIONAL_CONSTANT * mass) / Math.Pow((scale / 2), 2);


        //create genplanet object with parameters
        GenPlanet planet = new GenPlanet(prefab, new List<GameObject>(), new List<float>(), mass, planetRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed, false, true); ;

        return planet;
    }

    public GenPlanet CreatePlanet(int seed, Vector3 position, int index, float scale = -1.0f)
    {
        //create planet sphere with random scale and mass
        double mass = ((this.maxPlanetMass - this.minPlanetMass) * levelRandom.NextDouble() + this.minPlanetMass);

        //if no scale given then generate a random scale for planet
        if(scale == -1.0)
        {
            scale = (float) ((this.maxPlanetSize - this.minPlanetSize) * levelRandom.NextDouble() + this.minPlanetSize);
        }

        //generate random rotation speed
        double planetRotSpeed = ((this.maxPlanetRotationSpeed - this.minPlanetRotationSpeed) * levelRandom.NextDouble() + this.minPlanetRotationSpeed);
        planetRotSpeed *= planetRotationSpeedModifier;

        //calculate oribital speed (https://www.physicsclassroom.com/class/circles/Lesson-4/Mathematics-of-Satellite-Motion#:~:text=As%20seen%20in%20the%20equation,the%20orbit%20affect%20orbital%20speed)
        double orbitSpeed = Math.Sqrt((GRAVITATIONAL_CONSTANT * sunMass) / Vector3.Distance(sunPos, position));
        orbitSpeed *= oribitSpeedModifier;

        GameObject prefab = GenerateRandomPlanet();

        //calc gavity https://emandpplabs.nscee.edu/cool/temporary/doors/space/planets/find/findFg.html
        double gravity = (GRAVITATIONAL_CONSTANT * mass) / Math.Pow((scale / 2), 2);


        //create genplanet object with parameters
        GenPlanet planet = new GenPlanet(prefab, this.enemySpawnObjects[index], this.enemySpawnPercentages[index], mass, planetRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed, false, false);

        return planet;
    }

    public GenPlanet CreateAsteroid(int seed, Vector3 position, float scale = -1.0f)
    {
        //create planet sphere with random scale and mass
        double mass = ((this.maxPlanetMass - this.minPlanetMass) * levelRandom.NextDouble() + this.minPlanetMass);

        //if no scale given then generate a random scale for planet
        if (scale == -1.0)
        {
            scale = (float)((this.maxPlanetSize - this.minPlanetSize) * levelRandom.NextDouble() + this.minPlanetSize);
        }

        //generate random rotation speed
        double planetRotSpeed = ((this.maxPlanetRotationSpeed - this.minPlanetRotationSpeed) * levelRandom.NextDouble() + this.minPlanetRotationSpeed);
        planetRotSpeed *= planetRotationSpeedModifier;

        //calculate oribital speed (https://www.physicsclassroom.com/class/circles/Lesson-4/Mathematics-of-Satellite-Motion#:~:text=As%20seen%20in%20the%20equation,the%20orbit%20affect%20orbital%20speed)
        double orbitSpeed = Math.Sqrt((GRAVITATIONAL_CONSTANT * sunMass) / Vector3.Distance(sunPos, position));
        orbitSpeed *= oribitSpeedModifier;

        GameObject prefab = GenerateRandomPrefab(AsteroidPrefabs);

        //calc gavity https://emandpplabs.nscee.edu/cool/temporary/doors/space/planets/find/findFg.html
        double gravity = (GRAVITATIONAL_CONSTANT * mass) / Math.Pow((scale / 2), 2);


        //create genplanet object with parameters
        GenPlanet planet = new GenPlanet(prefab, this.enemySpawnObjects[0], this.enemySpawnPercentages[0], mass, planetRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed, false, false);

        return planet;
    }

    //creates new genplanet object for the shop ship in the solar system
    public GenPlanet CreateShopShip(int seed, Vector3 position, int index)
    {
        //dummy value for random mass of ship
        double mass = 100;

        //get scale for ship
        float scale = shopShipSize;

        //no orbit or rotation speed for ship in solar system (static)
        double shipRotSpeed = 0;
        double orbitSpeed = 0;

        //no gravity either
        double gravity = 0;

        //create genplanet object for easy creation in solar system but input no variables
        GenPlanet planet = new GenPlanet(shopShipPrefab, new List<GameObject>(), new List<float>(), mass, shipRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed, true, false);

        return planet;
    }

    public GenPlanet CreateSun(int seed, Vector3 position, int index, float scale = -1.0f)
    {
        //create planet sphere with random scale and mass
        this.sunMass = ((this.maxSunMass - this.minSunMass) * levelRandom.NextDouble() + this.minSunMass);

        //if no scale given then generate a random scale for planet
        if(scale == -1.0)
        {
            scale = (float) ((this.maxSunSize - this.minSunSize ) * levelRandom.NextDouble() + this.minSunSize);
        }

        //generate random rotation speed
        double planetRotSpeed = ((this.maxSunRotationSpeed - this.minSunRotationSpeed) * levelRandom.NextDouble() + this.minSunRotationSpeed);
        planetRotSpeed *= planetRotationSpeedModifier;

        //for now, since there will be only 1 sun, make orbit speed zero
        double orbitSpeed = 0;

        //generates random prefab from list
        GameObject prefab = GenerateRandomPrefab(SunPrefabs);
        //create sphere game object
        //create genplanet object with parameters
        //calc gavity https://emandpplabs.nscee.edu/cool/temporary/doors/space/planets/find/findFg.html
        double gravity = (GRAVITATIONAL_CONSTANT * this.sunMass) / Math.Pow((scale / 2), 2);


        //create genplanet object with parameters
        //enemy and percent are set to 0 to match with the Sun's index in the rest of this class
        GenPlanet sun = new GenPlanet(prefab, this.enemySpawnObjects[index], this.enemySpawnPercentages[index], this.sunMass, planetRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed, false, false);

        return sun;
    }

    
    private void AddSolarSystemComponentsToObj(GenPlanet planet)
    {
        planet.Planet.AddComponent<SolarSystemPlanet>();
        planet.Planet.GetComponent<SolarSystemPlanet>().Initialize(planet.mass, planet.radius, sunPos, planet.orbitSpeed, planet.rotationSpeed, planet.gravity);

        //add trigger sphere collider
        //Tala changed to sphere collider to make the trigger distance consistent
        if (planet.isBoss)
        {
            planet.Planet.AddComponent<BossCollider>();
        }
        else
        {
            planet.Planet.AddComponent<PlanetCollider>();
        }
        
        SphereCollider sphereCollider = planet.Planet.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        
        //set box collider scale to a certain meter width away from the planet
        //can do this by getting radius (new scale / 2) - (scale / 2)= width away from planet
        //new scale = 2 * (widthAway + (scale / 2))
        //float boxCollidersize = (float) (2 * (triggerBoxMetersAwayFromPlanet + planet.radius));
        //float boxColliderScale = (float) (boxCollidersize / (planet.planetScaleLocal));
        //sphereCollider.size = new Vector3(boxColliderScale,boxColliderScale,boxColliderScale);
        sphereCollider.radius = ((float)planet.radius + triggerBoxMetersAwayFromPlanet) / planet.planetScaleLocal;
        
    }

    private void AddSolarSystemComponentsToShop(GenPlanet shop)
    {

        SolarSystemPlanet solarSystemAtts = shop.Planet.AddComponent<SolarSystemPlanet>();
        solarSystemAtts.Initialize(shop.mass, shop.radius, sunPos, shop.orbitSpeed, shop.rotationSpeed, shop.gravity, true);


        //add trigger sphere collider
        //Tala changed to sphere collider to make the trigger distance consistent
        shop.Planet.AddComponent<ShopShipCollider>();
        SphereCollider sphereCollider = shop.Planet.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        sphereCollider.radius = ((float)shop.radius + triggerBoxMetersAwayFromPlanet) / shop.planetScaleLocal;
    }

    private GameObject GenerateRandomPrefab(List<GameObject> list)
    {
        int index = levelRandom.Next(0, list.Count);
        GameObject prefab = list[index];
        return prefab;
    }
    private GameObject GenerateRandomPlanet()
    {
        int count = PlanetPrefabs.Count;
        int index = levelRandom.Next(0, count);
        GameObject prefab = PlanetPrefabs[index];
        PlanetPrefabs.RemoveAt(index);
        
        if (PlanetPrefabs.Count == 0)
        {
            PlanetPrefabs.AddRange(PlanetPrefabsCopy); 
            PlanetPrefabsCopy.Clear();
        }
        PlanetPrefabsCopy.Add(prefab);
        return prefab;
    }
    public void CreateSolarSystemObjects()
    {
        int progressNum = 0;
        var progress = ProgressManager.GetInstance();
        //if there isnt a solar system made, make a solar system
        if(this.planetPositions.Count < 1)
        {
            SetPlanetAmount(6, 6);
            CreateSolarSystem();
        }

        //create sun
        Vector3 pos = this.planetPositions[0];
        GameObject ob = this.planets[0].CreatePlanetGameObject(pos, true);
        AddSolarSystemComponentsToObj(this.planets[0]);
        if(progress.IsPlanetCleared(ob))
        {
            WaypointManager.MakeWaypoint(this.planets[0].Planet, "cleared", "Cleared");
        }
        else
        {
            WaypointManager.MakeWaypoint(this.planets[0].Planet, "item", this.planets[0].upgrade.name);
        }
        progressNum++;
        this.planets[0].Planet.name = "Sun";

        //create planets/shop ship in a line like a solar system
        for(int i = 1; i < this.planets.Count; i++)
        {
            if(i == shopShipIndex)
            {
                pos = this.planetPositions[i];
                this.planets[i].CreatePlanetGameObject(pos);
                AddSolarSystemComponentsToShop(this.planets[i]);
                this.planets[i].Planet.name = "Shop Ship";
                WaypointManager.MakeWaypoint(this.planets[i].Planet, "shop", "Shop");
            }
            else if (i == bossIndex)
            {
                pos = this.planetPositions[i];
                GameObject obj = this.planets[i].CreatePlanetGameObject(pos, false, true);
                AddSolarSystemComponentsToObj(this.planets[i]);
                this.planets[i].Planet.name = "Boss";
                if(progress.IsPlanetCleared(obj))
                {
                    WaypointManager.MakeWaypoint(this.planets[i].Planet, "cleared", "Cleared");
                }
                else
                {
                    WaypointManager.MakeWaypoint(this.planets[i].Planet, "boss", "Boss");
                }  
                progressNum++;
            }
            else
            {
                pos = this.planetPositions[i];
                GameObject obj = this.planets[i].CreatePlanetGameObject(pos);
                AddSolarSystemComponentsToObj(this.planets[i]);
                this.planets[i].Planet.name = "Planet " + i;
                if(progress.IsPlanetCleared(obj))
                {
                    WaypointManager.MakeWaypoint(this.planets[i].Planet, "cleared", "Cleared");
                }
                else
                {
                    WaypointManager.MakeWaypoint(this.planets[i].Planet, "item", this.planets[i].upgrade.name);
                }  
                progressNum++;
            }
        }

        //Spawns in the asteroid belt
        var sun = planets[0];

        BeltSpawner belt = sun.Planet.gameObject.AddComponent<BeltSpawner>();
        belt.seed = levelSeed;
        belt.innerRadius = (planetPositions[planetPositions.Count - 1] - sun.Planet.transform.position).magnitude + 100f;
        belt.outerRadius = belt.innerRadius;
        // belt.asteroidPrefab = asteroid.CreatePlanetGameObject(new Vector3(0,0,0));
        belt.asteroidPrefabs = AsteroidPrefabs;
    }


}
