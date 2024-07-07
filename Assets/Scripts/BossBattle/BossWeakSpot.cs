using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public class BossWeakSpot : DamagableObject
{

    public static event Action<float> bossTakeDamage;

    public override void TakeDamage(float damageAmount, IPlayer player)
    {
        base.TakeDamage(damageAmount, player);
        bossTakeDamage?.Invoke(damageAmount);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
