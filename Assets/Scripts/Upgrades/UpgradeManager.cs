using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;


//this class houses all upgrades within a list
public static class UpgradeManager
{
    public static List<IUpgrade> PlanetPool;
    public static bool didInit = false;

    public static void Initialize()
    {
        PlanetPool = new List<IUpgrade>();
        didInit = true;

        InitPlanetUgradePool();
    }   

    //adds all upgrade objects to list
    private static void InitPlanetUgradePool()
    {
        PlanetPool.Add(new IncreaseDamagePercent());
        PlanetPool.Add(new IncreaseShotFan());
        PlanetPool.Add(new ControlledShot());
        PlanetPool.Add(new FlameShot());
        PlanetPool.Add(new Upgrades.GravityShot());
        PlanetPool.Add(new Upgrades.LightningShot());
        PlanetPool.Add(new PiercingShot());
        PlanetPool.Add(new SpectralShot());
        PlanetPool.Add(new Polyphemus());
        PlanetPool.Add(new BeamWeapon());
        PlanetPool.Add(new ExtraLife());
        PlanetPool.Add(new AddShot());
	}

    public static IUpgrade GetRandomPlanetPoolUpgrade(System.Random rnd)
    {
        int index = (int)rnd.Next(0, PlanetPool.Count);

        return PlanetPool[index];
    }
}
