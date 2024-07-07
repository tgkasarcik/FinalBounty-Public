using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;

//manages all projectile scripts (guns) for player
public class PlayerProjectiles
{
    private List<IGunType> gunScripts;
    public List<Type> guns {get;set;}
    private IPlayer player;

    public int currentGunIndex;
    public PlayerProjectiles(IPlayer player)
    {
        gunScripts = new List<IGunType>();
        this.player = player;
        currentGunIndex = -1;
    }

    //sets prefabs for projectiles
    public void InitProjectiles()
    {
        if(currentGunIndex == -1 && gunScripts.Count > 0)
        {
            currentGunIndex = 0;
        }
        int i = 0;
        foreach(IGunType gun in gunScripts)
        {
            gun.Initialize(player);
            if(currentGunIndex == i)
            {
                gun.isDisabled = false;
            }
            i++;
        }
    }

    public void AddGunsToPlayer(GameObject playerObj)
    {
        gunScripts = new List<IGunType>();
        if(currentGunIndex == -1 && guns.Count > 0)
        {
            currentGunIndex = 0;
        }
        for(int i = 0; i < guns.Count; i++)
        {
            IGunType gun = (IGunType)playerObj.AddComponent(guns[i]);
            gunScripts.Add(gun);
            gun.Initialize(player);
            if(currentGunIndex == i)
            {
                gun.isDisabled = false;
            }
        }

    }

    public void CycleGunUp()
    {
        currentGunIndex = (currentGunIndex + 1) % gunScripts.Count;
        EnableGun();
    }

    public void CycleGunDown()
    {
        currentGunIndex = (currentGunIndex - 1);
        if(currentGunIndex < 0)
        {
            currentGunIndex = gunScripts.Count - 1;
        }

        EnableGun();
    }

    public void SelectGun(int index)
    {
        currentGunIndex = index % gunScripts.Count;
        EnableGun();
    }

    public void SetPlanetObjects(GameObject planetObj)
    {
        foreach(IGunType gun in gunScripts)
        {
            gun.SetPlanetObj(planetObj);
        }
    }

    public void SetUseGravity(bool result)
    {
        foreach(IGunType gun in gunScripts)
        {
            gun.UseGravity = result;
        }
    }

    public void SetAttributes()
    {
        foreach(IGunType gun in gunScripts)
        {
            gun.SetAttributes();
        }
    }

    private void EnableGun()
    {
        int i = 0;
        foreach(IGunType gun in gunScripts)
        {
            if(currentGunIndex == i)
            {
                gun.isDisabled = false;
            }
            else
            {
                gun.isDisabled = true;
            }
            i++;
        }
    }

    public IGunType GetCurrentGun()
    {
        return gunScripts[currentGunIndex];
    }



}

