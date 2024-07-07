using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerObject;

public abstract class ITriggerShotCollider : MonoBehaviour
{
    public float damage = 1f;
    public bool flame = false;
    public bool piercing = false;
    public bool spectral = false;
    public bool isParticleSystem = false;
    public float flameDamage = 1f;
    private GameObject flameEffect;
    public IPlayer playerWhoOwnsShot;
	[SerializeField] public string TagToCheck;

	private GameObject hitParticle;
    public virtual void Awake()
    {
        hitParticle = (GameObject)Resources.Load("Prefabs/Particles/FX_BurstCloud");
        flameEffect = (GameObject)Resources.Load("Prefabs/Particles/FX_Fire_01");
    }

    public virtual void Start()
    {
        if(flame && !isParticleSystem)
        {
            GameObject flame = (GameObject)Resources.Load("Prefabs/Particles/FX_Fire_02");
            GameObject flameProjectile = Instantiate(flame, transform.position, transform.rotation);
            flameProjectile.transform.parent = this.transform;
            var rb = this.gameObject.GetComponent<Rigidbody>();
            var ps = flameProjectile.GetComponent<ParticleSystem>();
            var vel = ps.velocityOverLifetime;
            Vector3 targetVelocity = rb.velocity;
            vel.x = targetVelocity.x / ps.main.simulationSpeed;
            vel.y = targetVelocity.y / ps.main.simulationSpeed;
            vel.z = targetVelocity.z / ps.main.simulationSpeed;
        }

    }

    //damages enemies
    public virtual void ApplyOnHitEffects(Collider[] targets)
    {
        ApplyOnHitParticles(targets);
        for(int i = 0; i < targets.Length; i++)
        {   
            if(targets[i].tag == TagToCheck)
            {
                if(TagToCheck == "Player")
                {
                    //damage player
                    IPlayer objScript = PlayerManager.FindPlayerByObject(targets[i].gameObject);

                    if(!objScript.rolling)
                    {
                        objScript.TakeDamage(damage);

                        if(flame)
                        {
                            //apply flame FX (TODO: only put fire fx on non on fire enemies)

                            GameObject flameFX = Instantiate(flameEffect, targets[i].transform.position, targets[i].transform.rotation);
                            flameFX.transform.parent = targets[i].transform;
                            //apply flame damage (need to implement, for now just take extra damage)
                            objScript.TakeDamage(flameDamage);
                        }
                    }

                    //Projectiles will bounce away and become your own projectiles
                    else
                    {
                        var rb = GetComponent<Rigidbody>();
                        rb.velocity = -rb.velocity;
                        TagToCheck = "Enemy";
                        var destroyOnTouch = GetComponent<DestroyPrefabOnTouch>();
                        destroyOnTouch.TagToCheck = "Enemy";
                        CheckTagForColor(this.gameObject);

                        this.playerWhoOwnsShot = objScript;

                        //we should decide wether the projectile keeps its damage or reverts to the player's damage

                        //Not sure what this will do with other projectile types
                        //I think it will be fine
                    }
                    
                }
                else if(TagToCheck == "Enemy")
                {
                    //damage enemy
                    IEnemy objScript = targets[i].gameObject.GetComponent<IEnemy>();
                    int moneyOnDrop = objScript.moneyOnDrop;
                    if(flame)
                    {
                        //apply flame FX (TODO: only put fire fx on non on fire enemies)
                        if(!objScript.onFire)
                        {
                            GameObject flameFX = Instantiate(flameEffect, targets[i].transform.position, targets[i].transform.rotation);
                            flameFX.transform.parent = targets[i].transform;
                            //apply flame damage (need to implement, for now just take extra damage)
                            objScript.LightOnFire(0.5f, playerWhoOwnsShot);
                        }
                    }
                    objScript.TakeDamage(damage, playerWhoOwnsShot);
                }
            }
        }
    }

    public virtual void ApplyOnHitParticles(Collider[] targets)
    {
        foreach(Collider target in targets)
        {
            Instantiate(hitParticle, target.transform.position, target.transform.rotation);
        }
    }

    public abstract void CheckCollision(Collider collison);

    public void OnParticleCollision(GameObject other)
    {
        ParticleSystem currentParticle = this.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> particleCollisions = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(currentParticle, other, particleCollisions);
        for(int i = 0; i < particleCollisions.Count; i++)
        {
            Collider Collision = (Collider)particleCollisions[i].colliderComponent;
            if(Collision != null)
            {
                CheckCollision(Collision);
            }
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        CheckCollision(collision);
    }

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
    
}