using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;
using System.Linq;

public abstract class IGunType : MonoBehaviour
{
    //projectile data
    public int ammo {get;set;}

    public float damageOut {get;set;}
    public float damageOutPercent {get;set;}
    public int shotNumberOutputted {get;set;}
    public int projectileFanCount {get;set;}
    public float shootFOV {get;set;}
    public float projectileTime {get;set;}
    public float projectileSpeed {get;set;}
    public float projectileSize {get;set;}
    public float fireRate {get;set;}
    public float fireRatePercent {get;set;}

    public string TagToCheck {get; set;}

    public const int bufferValue = 1;
    public float gunHeat = 0;
    public bool isDisabled = true;

    public bool UseGravity {get;set;}

    public bool bossFightBehavior { get;set;}
    public GameObject Planet {get;set;}
    public IPlayer player;
    public IEnemy Enemy;
    public GameObject PlayerObj;

    public GameObject ProjectilePrefab {get;set;}
    public GameObject flameEffect {get;set;}

    public Quaternion stepAngle;
    public Quaternion FOV;
    public float step;
    public AudioManager _audioManager;

    public virtual void Awake()
    {
        ProjectilePrefab = (GameObject)Resources.Load("Prefabs/Projectiles/Projectile");
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void Initialize(IPlayer player, GameObject planet = null)
    {
        this.PlayerObj = this.gameObject;
        this.player = player;

        //if a planet isnt provided, assume projectiles travel on a plane not
        //a sphere
        if(planet == null)
        {
            this.UseGravity = false;
        }
        else
        {
            this.Planet = planet;
            this.UseGravity = true;
        }
        SetAttributes();

        TagToCheck = "Enemy";
    }

    //TODO: I will refactor this shit next week and make my code clean - will
    public void Initialize(IEnemy enemy, GameObject enemyObj, GameObject planet = null)
    {
        this.PlayerObj = enemyObj;
        this.Enemy = enemy;


        if(planet == null)
        {
            this.UseGravity = false;
        }
        else
        {
            this.Planet = planet;
            this.UseGravity = true;
        }

        SetEnemyAttributes();

        TagToCheck = "Player";
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    { 
        if(!isDisabled)
        {
            //https://forum.unity.com/threads/how-can-i-add-a-fire-rate-time-for-the-shooting-projectile.904001/
            if (gunHeat > 0)
            {
                gunHeat -= Time.deltaTime;
            }

            if (Input.GetMouseButton(0))
            {
                if (ammo > 0 && gunHeat <= 0)
                {
                    if (Camera.current != Camera.main)
                    {
                        if(this.GetType() != typeof(Lazer))
                        {
                            ammo--;
                        }

                        if(bossFightBehavior)
                        {
                            Shoot(player.playerObj.transform.forward, player.playerObj.transform.position, player.playerObj.transform);
                        }
                        else
                        {
                            Shoot();
                        }
                    }
                    gunHeat += 1 / (fireRate * fireRatePercent);
                }
            }
        }
    }

    public void AiFire(Vector3 direction, Vector3 position, Transform transform)
    {
        if (gunHeat > 0)
        {
            gunHeat -= Time.deltaTime;
        }

        if (ammo > 0 && gunHeat <= 0)
        {
                if (this.GetType() != typeof(Lazer))
                {
                    ammo--;
                }

                if (bossFightBehavior)
                {
                    Shoot(direction, position, transform);
                }
            gunHeat += 1 / (fireRate * fireRatePercent);
        }
    }


    public void SetPlanetObj(GameObject planet)
    {
        //Checks if the level is a boss level
        bossFightBehavior = planet.CompareTag("Boss") ? true : false;
        UseGravity = !bossFightBehavior;
        this.Planet = planet;
    }

    //player shooting method
    public virtual void Shoot()
    {
        Vector3 direction = GetDirection(PlayerObj);

        this.Shoot(direction, PlayerObj.transform.position, PlayerObj.transform);

    }

    //general Shooting method
    public virtual void Shoot(Vector3 direction, Vector3 position, Transform transform)
    {
        //step number for each run fan projectile creation
        this.step = shootFOV / projectileFanCount;
        Vector3 normalVector = transform.up;

        //angle to which the step increases by (angle in between each shot)
        this.stepAngle = Quaternion.AngleAxis(step, normalVector);

        //FOV of the player (as a quaternion)
        this.FOV = Quaternion.AngleAxis(-shootFOV / 2, normalVector);

        for (int r = 0; r < shotNumberOutputted; r++)
        {
            Vector3 startPos = position + (transform.right * (float)((1.5 * r - shotNumberOutputted / 2)));
            Vector3 shootDirection = direction;
            if (projectileFanCount != 1)
            {
                shootDirection = (Quaternion.Slerp(this.FOV, this.FOV * this.stepAngle, .5f)) * shootDirection;
            }
            //creates projectiles in a fan
            for (int i = 0; i < projectileFanCount; i++)
            {
                this.CreateProjectile(shootDirection, startPos, normalVector);
                shootDirection = this.stepAngle * shootDirection;
            }
        }
    }

    public virtual GameObject CreateProjectile(Vector3 direction, Vector3 startPos, Vector3 upVector)
    {
        GameObject projectile = Instantiate(ProjectilePrefab, startPos + (direction * bufferValue), Quaternion.identity);
        //Ignores the raycast for the boss level
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        projectile.layer = LayerIgnoreRaycast;
        projectile.transform.rotation = Quaternion.LookRotation(direction, upVector);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectile.transform.localScale = projectile.transform.localScale * projectileSize;
        Vector3 shootForce = direction * projectileSpeed; /*takes player's velocity into account: + PlayerObj.GetComponent<Rigidbody>().velocity; */
        projectileRb.AddForce(shootForce, ForceMode.VelocityChange);
        //wrap around planet if specified
        if(UseGravity)
        {
            GravityBody gravBod = projectile.AddComponent<GravityBody>();
            gravBod.SetPlanetObj(Planet);
        }
        return projectile;
    }

    public Vector3 GetDirection(GameObject PlayerObj)
    {
        //more hacky code, will clean up next week - will
        if (this.Enemy != null)
        {
            List<IPlayer> playerList = PlayerManager.players;
            IPlayer target = playerList.First();
            Vector3 directionEnemy = (PlayerObj.transform.position - target.playerObj.transform.position).normalized * -1;
            return directionEnemy;
        }
        //gets direction from 
        Vector3 mousePos = Input.mousePosition;
        Vector3 cameraToPlayerObj = Camera.main.transform.position - PlayerObj.transform.position;

        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(
             new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane + cameraToPlayerObj.magnitude));
        Vector3 direction = (clickPosition - PlayerObj.transform.position).normalized;
        return direction;
    }

    public virtual GameObject SetProjectileModifiers(GameObject projectile)
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

            var onTouch = projectile.AddComponent<DestroyPrefabOnTouch>();
            onTouch.TagToCheck = TagToCheck;
            onTouch.piercing = false;
            onTouch.spectral = false;

        }
        else
        {
            //hones in on enemies
            if(player.gravityShot)
            {
                var gravshot = projectile.AddComponent<GravityShot>();
                gravshot.sensingDistance = player.sensingDistance;
                gravshot.TagToCheck = TagToCheck;
            }

            //player can control projectile mid flight
            if(player.controledShot)
            {
                var ctrlShot = projectile.AddComponent<ControledShot>();
                //ctrlShot.PlayerObj = PlayerObj;
                ctrlShot.projectileSpeed = this.projectileSpeed;
                ctrlShot.Player = this.player;
            }

            var prefabTimer = projectile.AddComponent<DestroyPrefab>();
            prefabTimer.waitForTime = player.projectileTime;

            //chain lightning
            if(player.lightningShot)
            {
                var chainShot = projectile.AddComponent<LightningShot>();
                SetShotAttributes(chainShot);
                chainShot.damage = this.damageOut / 2;
            }

            //damages player hit
            var damageEnemy = projectile.AddComponent<DamageEnemy>();
            //init projectile variables
            SetShotAttributes(damageEnemy);

            var onTouch = projectile.AddComponent<DestroyPrefabOnTouch>();
            onTouch.TagToCheck = TagToCheck;
            onTouch.piercing = player.pierceShot;
            onTouch.spectral = player.spectralShot;
        }
        return projectile;
    }

    public virtual void SetAttributes()
    {
        this.damageOut = player.damageOut;
        this.damageOutPercent =  player.damageOutPercent;
        this.shotNumberOutputted = player.shotNumberOutputted;
        this.projectileFanCount =  player.projectileFanCount;
        this.shootFOV =  player.shootFOV;
        this.projectileTime =  player.projectileTime;
        this.projectileSpeed =  player.projectileSpeed;
        this.projectileSize =  player.projectileSize;
        this.fireRate =  player.fireRate;
        this.fireRatePercent =    player.fireRatePercent;
    }

    public virtual void SetShotAttributes(ITriggerShotCollider projectileScript)
    {
        projectileScript.playerWhoOwnsShot = player;
        projectileScript.TagToCheck = this.TagToCheck;
        projectileScript.damage = this.damageOut;
        projectileScript.flame = player.flameShot;
        projectileScript.piercing = player.pierceShot;
        projectileScript.spectral = player.spectralShot;
        projectileScript.flameDamage = player.flameDamage;
    }

    //TODO: I will refactor this shit next week and make my code clean - will
    public virtual void SetEnemyAttributes()
    {
        this.damageOut = 5f;
        this.damageOutPercent = 5f;
        this.shotNumberOutputted = 1;
        this.projectileFanCount = 1;
        this.shootFOV = 170f;
        this.projectileTime = 5f;
        this.projectileSpeed = 10f;
        this.projectileSize = 1f;
        this.fireRate = 4f;
        this.fireRatePercent = 1f;
    }
}