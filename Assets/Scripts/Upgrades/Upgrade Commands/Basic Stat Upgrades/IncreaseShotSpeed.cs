using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseShotSpeed : IUpgrade
    {
        public string name{get;} = "Projectile Speed Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.projectileSpeed += 5;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.projectileSpeed -= 5;
        }
    }
}
