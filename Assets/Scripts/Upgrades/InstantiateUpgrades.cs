using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateUpgrades : MonoBehaviour
{
    [SerializeField] public int Seed = -1;
    // gets all objects in scene with pickup tag and assigns an upgrade to it
    void Start()
    {
        if(!UpgradeManager.didInit)
        {
            UpgradeManager.Initialize();
        }

        System.Random rand;
        if(Seed == -1)
        {
            rand = new System.Random();
        }
        else
        {
            rand = new System.Random(PlayerLevelManager.GetCurrentLevelSeed());
        }

        GameObject[] upgradesInLevel = GameObject.FindGameObjectsWithTag("Pickup");
        foreach(GameObject upgradeObj in upgradesInLevel)
        {
            TouchUpgradeHandler upgradeTrigger = upgradeObj.GetComponent<TouchUpgradeHandler>();
            int index = rand.Next(UpgradeManager.PlanetPool.Count);
            upgradeTrigger.upgrade = UpgradeManager.PlanetPool[index];
        }
    }

}
