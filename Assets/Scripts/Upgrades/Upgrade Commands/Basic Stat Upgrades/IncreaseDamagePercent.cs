using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseDamagePercent : IUpgrade
    {
        public string name{get;} = "Damage Percent Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.damageOutPercent += .1f;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.damageOutPercent -= .1f;
        }
    }
}
