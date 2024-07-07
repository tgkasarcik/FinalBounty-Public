using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseDamage : IUpgrade
    {
        public string name{get;} = "Damage Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.damageOut += 1;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.damageOut -= 1;
        }
    }
}
