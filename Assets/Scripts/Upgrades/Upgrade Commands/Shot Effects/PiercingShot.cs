using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class PiercingShot : IUpgrade
    {
        public string name{get;} = "Piercing Shot";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.pierceShot = true;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.pierceShot = false;
        }
    }
}
