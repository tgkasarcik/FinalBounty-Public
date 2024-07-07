using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;

public class Lazer : IGunType
{
    private const float bufferValue = 1;
    private Vector3 prefabRotationAngles;
    //how many projectiles per second fired

    public override void Awake()
    {
        base.Awake();

        ProjectilePrefab = (GameObject)Resources.Load("Prefabs/Projectiles/Laser");
    }
    public override void Shoot()
    {
        base.Shoot();
        _audioManager.PlayRandomLazerProjectile();
    }

    //Using thhis method for the turrets
    public override void Shoot(Vector3 direction, Vector3 position, Transform transform)
    {

        base.Shoot(direction, position, transform);

        _audioManager.PlayRandomLazerProjectile();
    }

    public override GameObject CreateProjectile(Vector3 direction, Vector3 startPos, Vector3 upVector)
    {
        GameObject projectile = base.CreateProjectile(direction, startPos, upVector);

        projectile = base.SetProjectileModifiers(projectile);

        CheckTagForColor(projectile);

        return projectile;
    }

    public override void SetAttributes()
    {
        base.SetAttributes();
        this.ammo = 1;
    }

    //Changes the color if the tag is for a player or enemy
    public static void CheckTagForColor(GameObject projectile)
    {
        DamageEnemy damageEnemy = projectile.GetComponent<DamageEnemy>();
        if (damageEnemy.TagToCheck == "Enemy")
        {
            Material mat = projectile.GetComponentInChildren<MeshRenderer>().material;
            Light light = projectile.GetComponentInChildren<Light>();

            if (mat != null && light != null)
            {
                mat.color = Color.green;
                mat.SetColor("_EmissionColor", Color.green);
                light.color = Color.green;
            }
        }
    }


}