using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public static class PlayerManager
{
    public static List<IPlayer> players{get;set;}

    public static List<GameObject> playerPrefabs{get;set;}
    public static List<GameObject> projectilePrefabs{get;set;}

    private static System.Random rnd;

    public static bool didInit = false;

    private static AudioSource _engineSound;
    public static void Initialize(List<GameObject> playerPrefabsTemp, List<GameObject> projectilePrefabsTemp)
    {
        players = new List<IPlayer>();
        playerPrefabs = new List<GameObject>();
        rnd = new System.Random();
        playerPrefabs = playerPrefabsTemp;
        projectilePrefabs = projectilePrefabsTemp;
        didInit = true;

        _engineSound = GameObject.Find("EngineSource").GetComponent<AudioSource>();

    }

    //chooses random player prefab from prefabs and adds a player from it
    public static IPlayer AddNewPlayer(int playerIndex = -1, int projectileIndex = -1)
    {
        if(playerIndex == -1)
        {
            playerIndex = rnd.Next(0, playerPrefabs.Count);
        }
        if(projectileIndex == -1)
        {
            projectileIndex = rnd.Next(0, projectilePrefabs.Count);
        }
        IPlayer newPlayer = new PlayerShip(projectilePrefabs);
        newPlayer.prefab = playerPrefabs[playerIndex];
        players.Add(newPlayer);

        //_engineSound.Play();

        return newPlayer;
    }

    public static void RemovePlayer(int index)
    {
        players.RemoveAt(index);
    }

    public static void RemovePlayer(IPlayer playerToRemove)
    {
        players.Remove(playerToRemove);
    }

    public static void SpawnPlayers()
    {
        for(int i = 0; i < players.Count; i++)
        {
            //for now it spawns players at multiples of 5
            players[i].CreatePlayerObj(new Vector3((float)i * 5, 0, i * 5), Quaternion.identity);
            _engineSound.Play();
        }
    }

    public static IPlayer FindPlayerByObject(GameObject obj)
    {
        foreach(IPlayer player in players)
        {
            if(obj == player.playerObj)
            {
                return player;
            }
        }
        return null;
    }
}
