using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public class ShowPlanet : MonoBehaviour
{
    [SerializeField] public GameObject PlayerPrefab;

    [SerializeField] public GameObject ProjectilePrefab;
    [SerializeField] List<GameObject> enemies;
    [SerializeField] List<float> percents;
    private GameObject planet;
    private GameObject playerObj;
    private Gravity planetGrav;
    private GameObject mainPlayer;
    private AudioManager _audioManager;
    //locates planet that is shown, increases scale, and spawns enemies
    void Awake()
    {

        if(!UpgradeManager.didInit)
        {
            UpgradeManager.Initialize();
        }
        
        if(!PlanetRoomManager.didInit)
        {
            PlanetRoomManager.Initialize();
            //creates solar system and 
            PlanetRoomManager.SetPlanetAmount(4, 6);
            PlanetRoomManager.currentPlanet = PlanetRoomManager.CreateSolarSystem()[2];
        }
        //make scale bigger
        PlanetRoomManager.currentPlanet.planetScaleLocal  *= PlanetRoomManager.planetUpscale;
        PlanetRoomManager.currentPlanet.planetScaleGlobal  *= PlanetRoomManager.planetUpscale;

        planet = PlanetRoomManager.CreateCurrentPlanet(new Vector3(0,0,0));
        //planet = PlanetRoomManager.currentPlanet.Planet;
        planet.tag = PlanetRoomManager.currentPlanet.Planet.tag;
        planet.name = PlanetRoomManager.currentPlanet.Planet.name;
        //PlanetRoomManager.currentPlanet.Enemy = PlanetRoomManager.Enemies;
        //PlanetRoomManager.currentPlanet.Enemy = PlanetRoomManager.Enemies;
        //PlanetRoomManager.currentPlanet.EnemySpawnPercent = PlanetRoomManager.spawnPercents;
        //PlanetRoomManager.currentPlanet.EnemySpawnPercent = PlanetRoomManager.spawnPercents;

        //spawn enemies
        PlanetRoomManager.currentPlanet.LoadPrefabDictionary();
        PlanetRoomManager.currentPlanet.LoadSpawns();
        PlanetRoomManager.currentPlanet.SpawnThings();
        //creates player in scene and sets variables if player manager is initialized
        if(!PlayerManager.didInit)
        {
            PlayerManager.Initialize(new List<GameObject> {PlayerPrefab}, new List<GameObject> {ProjectilePrefab});
            PlayerManager.AddNewPlayer();
        }
        
        //spawn in players
        //bool isMainPlayerDecided = false;

        foreach(IPlayer player in PlayerManager.players)
        {
            player.playerObj = PlanetRoomManager.currentPlanet.LoadInPlayer(player.prefab);
            player.InitPlayerAttributes();
            player.playerMovement.orbiting = true;

            //Makes the player face the boss
            if(PlanetRoomManager.currentPlanet.isBoss)
            {
                player.playerMovement.bossBattle = true;
                player.bossBattle = true;
            }

            // if(!isMainPlayerDecided)
            //  {
            //     mainPlayer = playObj;
            // }

            // get gravityBody component and fill out variables

            var gravBod = player.playerObj.GetComponent<GravityBody>();

            gravBod.SetPlanetObj(planet);

            player.SetOnPlanet(true);
        }

        mainPlayer = PlayerManager.players[0].playerObj;
        
        //get camera controller and fill out variables
        GameObject camera = GameObject.Find("Main Camera");
        CameraController controller = camera.GetComponent<CameraController>();
        controller.Sphere = planet;
        controller.Player = mainPlayer;

        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        _audioManager.StartFadeGameMusicOut();
        _audioManager.FadeRandomCombatMusicIn();
    }

}
