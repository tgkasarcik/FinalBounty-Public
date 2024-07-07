using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : ITriggerShotCollider
{
    private List<Collider> previousEnemiesHit;
    public virtual void Awake()
    {
        base.Awake();
        previousEnemiesHit = new List<Collider>();
    }

    public override void CheckCollision(Collider collision)
    {
        if(collision.tag == TagToCheck)
        {
            Collider[] targets = {collision};
            if(piercing)
            {
                if(!previousEnemiesHit.Contains(collision)){
                    //damage enemy
                    ApplyOnHitEffects(targets);
                    previousEnemiesHit.Add(collision);
                }
            }
            else
            {
                //damage enemy
                ApplyOnHitEffects(targets);
            }

        }
    }

}
