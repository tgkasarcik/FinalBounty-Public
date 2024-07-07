using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionShot : ITriggerShotCollider
{
    public float explosionRadius = 5f;
    private GameObject explosionFX;
    private string actualTagToCheck;
    private AudioManager _audioManager;

    public override void Awake()
    {
        actualTagToCheck = "Enemy";
        base.Awake();
        explosionFX = (GameObject)Resources.Load("Prefabs/Particles/FX_Explosion");
        _audioManager = FindObjectOfType<AudioManager>();
	}

    public override void CheckCollision(Collider collision)
    {
        //this is here so that the check to explode is against this tag (as rockets should explode on everything
        // except the thing shooting it) 
        if (TagToCheck == "Enemy")
        {
            actualTagToCheck = "Player";
        }
        else
        {
            actualTagToCheck = "Enemy";
        }

        if(collision.tag != actualTagToCheck && collision.tag != "Cell" && collision.tag != "Projectile" && collision.tag != "Untagged")
        {
            //Dont want the rockets to explode when the turret rockets hit the boss
            if(!(actualTagToCheck == "Enemy" && collision.tag == "Boss"))
            {
                //explode
                Collider[] targets = Physics.OverlapSphere(this.gameObject.transform.position, explosionRadius);
                ApplyOnHitEffects(targets);
                _audioManager.PlayRandomExplosion();
            }
        }
    }

    public override void ApplyOnHitParticles(Collider[] targets)
    {
        Instantiate(explosionFX, transform.position, transform.rotation);
    }

}
