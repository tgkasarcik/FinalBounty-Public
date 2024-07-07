using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetGen;

public static class EnemyCounter 
{   
    private static int EnemyCount;
    private static GenPlanet currentPlanet;
    private static bool isEnabled;

    static EnemyCounter() 
    {
        EnemyCount = 0;
    }  

    public static void SetPlanet(GenPlanet planet)
    {
        currentPlanet = planet;
        EnemyCount = 0;
    }

    public static void AddEnemyCount()
    {
        EnemyCount++;
        
    }
    public static void MinusEnemyCount()
    {
        if(EnemyCount > 0)
        {
            EnemyCount--;

            if(EnemyCount == 0 && currentPlanet != null && currentPlanet.spawnsInit && PlayerManager.players[0].onPlanet
            && PlayerManager.players[0].playerObj != null && !currentPlanet.isBoss)
            {
                //very hacky but im so tired lmoa
                currentPlanet.SetCleared();
            }
        }
        //For testing 
        //EnemyCount = 0;

    }
    public static Boolean IsCleared()
    {
        return EnemyCount >= 0;
    }
}   
