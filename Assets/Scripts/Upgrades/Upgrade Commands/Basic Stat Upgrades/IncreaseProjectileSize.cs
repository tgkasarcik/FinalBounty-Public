using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseProjectileSize : IUpgrade
    {
        public string name{get;} = "Projectile Size Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.projectileSize += .4f;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.projectileSize -= .4f;
        }
    }
}
