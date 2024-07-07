using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShot : MonoBehaviour
{
   public float sensingDistance = 5f;
   public float projectileSpeed = 1f;
   public string TagToCheck = "Enemy";
   private const float checkTime = .2f;
   private float timer;
   private GameObject target;
   private Rigidbody projectileRb;
   void Start()
   {
        projectileRb = this.gameObject.GetComponent<Rigidbody>();
        timer = checkTime;
   }
   void FixedUpdate()
   {
        //detect enemies if target is null in certain time intervals
        if(timer <= 0)
        {
            target = findEnemy();
            timer += checkTime;
        }
        timer -= Time.deltaTime;
        
        if(target != null)
        {
            Vector3 direction = Vector3.Normalize(target.transform.position - this.transform.position);
            //Vector3 direction = target.transform.position - this.gameObject.transform.position;
            projectileRb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
            this.transform.rotation = Quaternion.LookRotation(projectileRb.velocity, this.transform.up);
            //float angleDifference = Vector3.Angle(facing, velocity);
        }
   }

    //finds enemies within range of projectile
   private GameObject findEnemy()
   {
        Collider[] targets = Physics.OverlapSphere(this.gameObject.transform.position, sensingDistance);
        foreach(Collider potentialEnemy in targets)
        {
            if(potentialEnemy.tag == TagToCheck)
            {
                return potentialEnemy.gameObject;
            }
        }

        return null;
   }
}
