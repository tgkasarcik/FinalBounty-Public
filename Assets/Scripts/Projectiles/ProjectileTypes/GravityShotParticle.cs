using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShotParticle : MonoBehaviour
{
   public float sensingDistance = 5f;
   public float projectileSpeed = 5f;
   public string TagToCheck = "Enemy";
   private const float checkTime = .2f;
   private float timer;
   private GameObject target;
   ParticleSystem.ForceOverLifetimeModule forceSystem;
   private ParticleSystem ps;
   private float forceX, forceY, forceZ;
   private bool first = true;

   void Start()
   {
        ps = this.gameObject.GetComponent<ParticleSystem>();
        forceSystem = ps.forceOverLifetime;
        timer = checkTime;
        forceX = forceSystem.x.constant;
        forceY = forceSystem.y.constant;
        forceZ = forceSystem.z.constant;

   }
   void FixedUpdate()
   {
        // forceSystem.x = forceSystem.x.constant;
        // forceSystem.y = forceY;
        // forceSystem.z = forceZ;
        //detect enemies if target is null in certain time intervals
        if(timer <= 0)
        {
            target = findEnemy();
            timer += checkTime;
        }
        timer -= Time.deltaTime;
        
        if(target != null)
        {
            //Vector3 direction = Vector3.Normalize(target.transform.position - this.transform.position);
            Vector3 direction = target.transform.position - this.gameObject.transform.position;
            Vector3 directionalForce = direction * projectileSpeed * 50;
            forceSystem.x = forceSystem.x.constant + directionalForce.x;
            forceSystem.y = forceSystem.y.constant + directionalForce.y;
            forceSystem.z = forceSystem.z.constant + directionalForce.z;
            
            //this.transform.rotation = Quaternion.LookRotation(projectileRb.velocity, this.transform.up);
            //float angleDifference = Vector3.Angle(facing, velocity);
        }
   }

    //finds enemies within range of projectile
   private GameObject findEnemy()
   {
        //TODO: maybe find a way to make overlap box curve around planets instead of having it just be taller
        // as a temporary fix
        Vector3 boxSpecs = new Vector3(sensingDistance /2, 20, projectileSpeed * 10);
        Vector3 center = this.transform.position + projectileSpeed * this.transform.forward * 5;
        Quaternion rotation = Quaternion.LookRotation(this.transform.forward, this.transform.up);

        // first = false;
        // if(first)
        // {
        //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //     cube.transform.localScale = boxSpecs;
        //     cube.transform.position = center;
        //     cube.transform.rotation = rotation;
        //     cube.transform.parent = this.transform;
        //     first = false;
        // }
        //Debug.Log(boxSpecs);
        //Debug.Log(center);

        Collider[] targets = Physics.OverlapBox(center, boxSpecs, rotation);
        //Collider[] targets = Physics.OverlapSphere(this.transform.position + this.transform.forward * 2, sensingDistance * 2);
        foreach(Collider potentialEnemy in targets)
        {
            //Debug.Log(potentialEnemy.tag);
            if(potentialEnemy.tag == TagToCheck)
            {
                //Debug.Log(potentialEnemy.tag);
                return potentialEnemy.gameObject;
            }
        }

        return null;
   }
}
