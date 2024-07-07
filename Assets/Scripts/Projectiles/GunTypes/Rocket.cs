using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;

public class Rocket : IGunType
{
    private const float bufferValue = 1;
    //how many projectiles per second fired

    public override void Awake()
    {
        base.Awake();
        ProjectilePrefab = (GameObject)Resources.Load("Prefabs/Projectiles/Rocket");
    }
    

    public override void Shoot()
    {
        base.Shoot();
        player.rocketAmmo--;
    }

    public override GameObject CreateProjectile(Vector3 direction, Vector3 startPos, Vector3 upVector)
    {
        GameObject projectile = base.CreateProjectile(direction, startPos, upVector);

        projectile = SetProjectileModifiers(projectile);
        GameObject flameFx = (GameObject)Resources.Load("Prefabs/Particles/FX_RocketFlame");
        GameObject flameTrail = Instantiate(flameFx, projectile.transform.position, projectile.transform.rotation);
        flameTrail.transform.parent = projectile.transform;
        


        return projectile;
    }

    public override GameObject SetProjectileModifiers(GameObject projectile)
    {
        projectile = base.SetProjectileModifiers(projectile);

        var explosion = projectile.AddComponent<ExplosionShot>();
        SetShotAttributes(explosion);
        //Debug.Log(explosion.TagToCheck);
        return projectile;
    }


    //sets rockets to be more powerful but slower
    public override void SetAttributes()
    {
        this.ammo = player.rocketAmmo;
        this.damageOut = player.damageOut * 2.5f;
        this.damageOutPercent =  player.damageOutPercent * 1.1f;
        this.shotNumberOutputted = player.shotNumberOutputted;
        this.projectileFanCount = player.projectileFanCount;
        this.shootFOV =  player.shootFOV;
        this.projectileTime =  player.projectileTime * 1.5f;
        this.projectileSpeed =  player.projectileSpeed * 0.85f;
        this.projectileSize =  player.projectileSize;
        this.fireRate =  player.fireRate * 0.25f;
        this.fireRatePercent =   player.fireRatePercent;
    }

    public override void Shoot(Vector3 direction, Vector3 position, Transform transform)
    {
        base.Shoot(direction, position, transform);
        _audioManager.PlayRandomMissileFire();
    }
}