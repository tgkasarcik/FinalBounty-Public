using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class GravityShot : IUpgrade
    {
        public string name{get;} = "Gravity Shot";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.gravityShot = true;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.gravityShot = false;
        }
    }
}
