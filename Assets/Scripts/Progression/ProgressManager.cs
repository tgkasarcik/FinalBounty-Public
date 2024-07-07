using PlanetGen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager
{
    //stores planet and level progression data
    private static ProgressManager instance;
    //each array index is a level and the genplanets are what is in that level.
    private static HashSet<GenPlanet>[] LevelPlanets;
    private static Boolean Win;
    private static Boolean[] LevelCleared;
    static ProgressManager()
    {
        Win = false;
        LevelPlanets = new HashSet<GenPlanet>[3];
        for(int i = 0; i < LevelPlanets.Length; i++)
        {
            LevelPlanets[i] = new HashSet<GenPlanet>();
        }
        //new stuff
        LevelCleared = new bool[3] {false,false,false};
    }
    //Singleton
    public static ProgressManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ProgressManager();
        }
        return instance;
    }
    public void SetLevelCleared()
    {
        var level = PlayerLevelManager.GetCurrentLevel();
        LevelCleared[level] = true;
    }
    public void SetPlanetCleared()
    {
        
        LevelPlanets[PlayerLevelManager.GetCurrentLevel()].Add(PlanetRoomManager.currentPlanet);
    }

    public Boolean IsPlanetCleared(GameObject Planet)
    {
        PlanetRoomManager.FindGenPlanet(Planet);
        return LevelPlanets[PlayerLevelManager.GetCurrentLevel()].Contains(PlanetRoomManager.FindGenPlanet(Planet));
    }
    public Boolean IsLevelCleared(int Level)
    {
        return LevelCleared[Level];
    }
    public void setWin(bool win)
    {
        Win = win;
    }
    public Boolean IsWon()
    {
        return Win;
    }
}
