using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public class LevelCreation : MonoBehaviour
{
    [SerializeField] public int Seed = -1;
    [SerializeField] public int MinPlanets = 6;
    [SerializeField] public int MaxPlanets = 10;
    [SerializeField] public List<GameObject> PlanetPrefabs;
    [SerializeField] public List<GameObject> SunPrefabs;
    [SerializeField] public List<GameObject> BossPrefabs;
    [SerializeField] public List<GameObject> AsteroidPrefabs;
    [SerializeField] public GameObject PlayerPrefab;

    [SerializeField] public GameObject ProjectilePrefab;

    private AudioManager _audioManager;

    void Awake()
    {
        if(!PlanetRoomManager.didInit)
        {
            //instantiates player room manager
            PlanetRoomManager.Initialize(PlayerLevelManager.GetCurrentLevelSeed());
            PlanetRoomManager.SetPlanetPrefabs(PlanetPrefabs, SunPrefabs, BossPrefabs, AsteroidPrefabs);
            PlanetRoomManager.SetPlanetAmount(MinPlanets, MaxPlanets);
            //creates solar system for scene (genplanet objects + gameObjects)
            PlanetRoomManager.CreateSolarSystem();
        }

        //makes upgrade instantiation seed the same as level seed
        var upgrades = this.gameObject.GetComponent<InstantiateUpgrades>();
        upgrades.Seed = PlayerLevelManager.GetCurrentLevelSeed();


        //spawns players in scene
        if(!PlayerManager.didInit)
        {
            //if not initialized, initializes playermanager with player and projectile prefabs listed in script object
            PlayerManager.Initialize(new List<GameObject> {PlayerPrefab}, new List<GameObject> {ProjectilePrefab});
            PlayerManager.AddNewPlayer();
        }

        PlayerManager.SpawnPlayers();


        //this is used for who the camera follows
        GameObject playerOne = PlayerManager.players[0].playerObj;

        //set camera variables
        var camera = GameObject.FindWithTag("MainCamera");
        camera.transform.position = playerOne.transform.position + new Vector3(0, 50f, -15f);
        camera.GetComponent<CameraControllerLegacy>().Player = playerOne;

        _audioManager = FindObjectOfType<AudioManager>();

        _audioManager.StartFadeTitleMusicOut();
        _audioManager.StartFadeCombatMusicOut();
        _audioManager.StartFadeGameMusicIn();
        //_audioManager.StartCombatMusic();
        //_audioManager.StartCombatMusic();
        //_audioManager.StartFadeCombatMusicIn();

       /* desired code, need to fix 
        * _audioManager.StartFadeTitleMusicOut();
        _audioManager.StartFadeCombatMusicIn();*/
    }

    void Start()
    {
        PlanetRoomManager.CreateSolarSystemObjects();
        
        foreach(IPlayer player in PlayerManager.players)
        {
            GameObject playerObject = player.playerObj;
            playerObject.transform.position = PlanetRoomManager.solarSystem.planetPositions[0] + new Vector3((float)PlanetRoomManager.solarSystem.planets[0].radius + 10, 0, 0);
            playerObject.GetComponent<GravityBody>().enabled = false;
            var rb = playerObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        }
    }
}
