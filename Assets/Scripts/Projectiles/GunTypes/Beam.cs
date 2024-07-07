using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;

public class Beam : IGunType
{
    private const float bufferValue = 1;
    private const int emittedParticleNum = 1;
    private Vector3 prefabRotationAngles;

    private float ammoTimer = 0f;
    private float ammoTime = .01f;

    //amount of time beam fires for in seconds
    private float beamUpTimer = 2;
    private float realBeamUpTimer;

    private bool fire;

    private List<GameObject> BeamParticles;
    private int index;

    private AudioManager _audioManager;
    //how many projectiles per second fired

    public virtual void FixedUpdate()
    { 
        if(!isDisabled)
        {
            UpdateParticleEmitters();
            if(fire)
            {
                //fires beam if at 100 ammo until ammo is depleted
                if (Camera.current != Camera.main)
                {
                    if(ammo == 100)
                    {
                        _audioManager.PlayBeamShoot();
                    }
                    if(bossFightBehavior)
                    {
                        Shoot(player.playerObj.transform.forward, player.playerObj.transform.position, player.playerObj.transform, player.projectileSpeed);
                    }
                    else
                    {
                        Shoot();
                    }
                }

                //subtracts an ammo if 100th of a 
                gunHeat += Time.deltaTime;
                while(gunHeat > realBeamUpTimer)
                {
                    ammo--;
                    gunHeat -= realBeamUpTimer;
                }

                if(ammo <= 0)
                {
                    fire = false;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if(ammo == 0)
                {
                    _audioManager.PlayBeamCharge(realBeamUpTimer * 100);
                }
                if(ammo < 100 )
                {
                    //adds 1 ammo per time allotted to charge up beam
                    ammoTimer += Time.deltaTime;
                    while(ammoTimer >= ammoTime)
                    {
                        ammo++;
                        ammoTimer -= ammoTime;
                    }
                }
            }
            else if(ammo >= 100)
            {
                fire = true;
            }
            else 
            {
                //reset charge up if not held
                ammo = 0;
                _audioManager.StopBeamCharge();
            }

            player.beamAmmo = ammo;
        }
    }

    public override void Awake()
    {
        base.Awake();
        fire = false;
        ProjectilePrefab = (GameObject)Resources.Load("Prefabs/Projectiles/FX_LazerBeam");
        index = 0;
        BeamParticles = new List<GameObject>();
        realBeamUpTimer = beamUpTimer / 100;
        // var vel = partSystem.velocityOverLifetime;
        // var velocityScale = (2.5 / vel.speedModifier) * ChainLightningFX.transform.localScale;
        // vel.x = targetVelocity.x / velocityScale;
        // vel.y = targetVelocity.y / velocityScale;
        // vel.z = targetVelocity.z / velocityScale;
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public override void Shoot()
    {
        Vector3 direction = GetDirection(PlayerObj);
        this.Shoot(direction, PlayerObj.transform.position, PlayerObj.transform, player.projectileTime);
    }

    public void Shoot(Vector3 direction, Vector3 position, Transform transform, float Distance)
    {
        index = 0;

        //step number for each run fan projectile creation
        this.step = shootFOV/projectileFanCount;
        Vector3 normalVector;
        
        if(UseGravity || bossFightBehavior)
        {
            //normalVector = new Vector3(PlayerObj.transform.up.x, Math.Abs(PlayerObj.transform.up.y),PlayerObj.transform.up.z);
            normalVector = transform.up;
            //normalVector = Vector3.up;
        }
        else
        {
            normalVector = Vector3.up;
        }

        //angle to which the step increases by (angle in between each shot)
        this.stepAngle = Quaternion.AngleAxis(step, normalVector);

        //FOV of the player (as a quaternion)
        this.FOV = Quaternion.AngleAxis(-shootFOV / 2, normalVector);

        for(int r = 0; r < shotNumberOutputted; r++)
        {
            Vector3 startPos = position + (transform.right * (float)((1.5 * r - shotNumberOutputted /2)));
            Vector3 shootDirection = direction;
            if(projectileFanCount != 1)
            { 
                shootDirection = (Quaternion.Slerp(this.FOV, this.FOV * this.stepAngle, .5f)) * shootDirection;
            }
            //creates projectiles in a fan
            for(int i = 0; i < projectileFanCount; i++)
            {
                this.CreateProjectile(shootDirection, startPos, normalVector, Distance);
                shootDirection = this.stepAngle * shootDirection;
                index++;
            }
        }
        //subtract ammo for cooldown
    }

    public GameObject CreateProjectile(Vector3 direction, Vector3 startPos, Vector3 upVector, float distance)
    {
        //GameObject projectile = Instantiate(ProjectilePrefab, startPos + (direction * bufferValue), Quaternion.identity);
        GameObject BeamParticle = BeamParticles[index];
        BeamParticle.transform.position = startPos + (direction * bufferValue);
        BeamParticle.transform.rotation = Quaternion.LookRotation(direction, upVector);
        ParticleSystem partSystem = BeamParticles[index].GetComponent<ParticleSystem>();
        Vector3 targetVelocity = direction * distance / 2;
        partSystem.Emit(emittedParticleNum);
        var vel = partSystem.velocityOverLifetime;
        //var vel = partSystem.forceOverLifetime;
        var velocityScale = (float)(2.5 / vel.speedModifierMultiplier) * ProjectilePrefab.transform.localScale;
        //var velocityScale = ProjectilePrefab.transform.localScale;
        vel = partSystem.velocityOverLifetime;
        vel.x = targetVelocity.x / velocityScale.x;
        vel.y = targetVelocity.y / velocityScale.y;
        if(UseGravity || bossFightBehavior)
        {
            vel.y = targetVelocity.y / velocityScale.y;
        }
        else
        {
            vel.y = 0;
        }
        vel.z = targetVelocity.z / velocityScale.z;
        //wrap around planet if specified


        //CheckTagForColor(projectile);

        return null;
    }

    //adds or removes lazer beams accordingly
    public void UpdateParticleEmitters()
    {

        //adds/takes away new particles if not correct number
        int totalEmitters = shotNumberOutputted * projectileFanCount;
        while(BeamParticles.Count != totalEmitters)
        {
            if(BeamParticles.Count < totalEmitters)
            {
                //add if too little
                GameObject BeamParticle = Instantiate(ProjectilePrefab, transform.position, transform.rotation);
                //BeamParticle.name = "LazerBeamParticle";
                BeamParticle.transform.parent = this.transform;
                BeamParticles.Add(BeamParticle);
                SetProjectileModifiers(BeamParticle);
            }
            else
            {
                //remove if too many
                BeamParticles.RemoveAt(BeamParticles.Count - 1);
            }
        }

        //updates size of particles
        foreach(GameObject BeamParticle in BeamParticles)
        {
            ParticleSystem partSys = BeamParticle.GetComponent<ParticleSystem>();
            var main = partSys.main;
            main.startSize = this.projectileSize;

            var coll = partSys.collision;
            if(this.player.spectralShot)
            {
                LayerMask enemyLayer = LayerMask.GetMask("EnemiesDontCollide");
                coll.collidesWith = enemyLayer;
            }

            if(this.player.pierceShot)
            {
                coll.maxKillSpeed = 10000000;
            }
        }
    }

    public override GameObject SetProjectileModifiers(GameObject projectile)
    {
        //more hacky code, will clean up next week - will
        if (this.Enemy != null)
        {
            var prefabTimer = projectile.AddComponent<DestroyPrefab>();
            prefabTimer.waitForTime = 15;

            //TODO: damage player!
            var damageEnemy = projectile.AddComponent<DamageEnemy>(); 
            damageEnemy.TagToCheck = TagToCheck;
            damageEnemy.damage = 3f;
            //init projectile variables
            //damageEnemy.piercing = false;


        }
        else
        {
            if(player.gravityShot)
            {
                var gravshot = projectile.AddComponent<GravityShotParticle>();
                gravshot.sensingDistance = player.sensingDistance;
                gravshot.projectileSpeed = this.projectileTime;
                gravshot.TagToCheck = TagToCheck;
            }

            //dont need controlled shot because lazer beam already goes immediatly to the cursor

            if(player.lightningShot)
            {
                var chainShot = projectile.AddComponent<LightningShot>();
                SetShotAttributes(chainShot);
                chainShot.isParticleSystem = true;
                chainShot.damage = this.damageOut / 2;
            }

            var damageEnemy = projectile.AddComponent<DamageEnemy>();
            //init projectile variables
            SetShotAttributes(damageEnemy);
            damageEnemy.isParticleSystem = true;

        }

        if(UseGravity)
        {
            var gravBod = projectile.AddComponent<GravityBodyParticle>();
            gravBod.SetPlanetObj(Planet);
        }

        return projectile;
    }

    //Changes the color if the tag is for a player or enemy
    public void CheckTagForColor(GameObject projectile)
    {
        DamageEnemy damageEnemy = projectile.GetComponent<DamageEnemy>();
        if (damageEnemy.TagToCheck == "Enemy")
        {
            Material mat = projectile.GetComponentInChildren<MeshRenderer>().material;
            Light light = projectile.GetComponentInChildren<Light>();

            mat.color = Color.green;
            mat.SetColor("_EmissionColor", Color.green);
            light.color = Color.green;
        }
    }

    //sets rockets to be more powerful but slower
    public override void SetAttributes()
    {
        this.ammo = 0;
        float dmgPerParticle = player.damageOut / emittedParticleNum;
        this.damageOut = dmgPerParticle *.3f;
        this.damageOutPercent =  player.damageOutPercent;
        this.shotNumberOutputted = player.shotNumberOutputted;
        this.projectileFanCount =  player.projectileFanCount;
        this.shootFOV =  player.shootFOV;
        this.projectileTime =  player.projectileTime * 1.5f;
        this.projectileSpeed =  player.projectileSpeed * 0.85f;
        this.projectileSize =  player.projectileSize;
        this.fireRate =  player.fireRate * 0.15f;
        this.fireRatePercent =   player.fireRatePercent;
        this.ammoTime = (1/ (this.fireRate * this.fireRatePercent)) / 100;
    }


}