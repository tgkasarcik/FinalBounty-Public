using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningShot : ITriggerShotCollider
{
    public float chainRadius = 10f;
    private GameObject ElectricityFX;
    private GameObject ChainLightningFX;
    public override void Awake()
    {
        base.Awake();
        ElectricityFX = (GameObject)Resources.Load("Prefabs/Particles/FX_Electricity");
        ChainLightningFX = (GameObject)Resources.Load("Prefabs/Particles/FX_ChainLightning");
        //make explosion FX
        GameObject electricity = Instantiate(ElectricityFX, transform.position, transform.rotation);
        electricity.transform.parent = this.transform;
    }

    public override void CheckCollision(Collider collision)
    {
        if(collision.tag == TagToCheck)
        {
            List<Collider> targets = GetTargets(collision, new List<Collider>());
            targets.Remove(collision);
            ApplyOnHitEffects(targets.ToArray());
        }
    }

    private List<Collider> GetTargets(Collider enemy, List<Collider> hitEnemies)
    {
        hitEnemies.Add(enemy);
        Vector3 enemyPos = enemy.transform.position;
        Collider[] targets = Physics.OverlapSphere(enemyPos, chainRadius);
        foreach(Collider target in targets)
        {
            if(target.tag == TagToCheck && !hitEnemies.Contains(target))
            {
                Vector3 targetVelocity = target.transform.position - enemy.transform.position;
                GameObject chainElectricity = Instantiate(ChainLightningFX, enemy.transform.position, Quaternion.identity);
                var partSystem = chainElectricity.GetComponent<ParticleSystem>();
                var vel = partSystem.velocityOverLifetime;
                var velocityScale = (float)(2.5 / vel.speedModifierMultiplier) * ChainLightningFX.transform.localScale * .7f;
                vel.x = targetVelocity.x / velocityScale.x;
                vel.y = targetVelocity.y / velocityScale.y;
                vel.z = targetVelocity.z / velocityScale.z;
                hitEnemies = GetTargets(target, hitEnemies);
            }
        }
        return hitEnemies;
    }

}
