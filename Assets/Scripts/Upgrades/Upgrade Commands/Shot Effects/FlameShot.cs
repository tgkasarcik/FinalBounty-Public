using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class FlameShot : IUpgrade
    {
        public string name{get;} = "Flame Shot";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.flameShot = true;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.flameShot = false;
        }
    }
}
