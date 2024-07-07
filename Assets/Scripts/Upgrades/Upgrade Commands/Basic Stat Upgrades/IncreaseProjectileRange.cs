using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseProjectileRange : IUpgrade
    {
        public string name{get;} = "Projectile Range Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.projectileTime += 2;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.projectileTime -= 2;
        }
    }
}
