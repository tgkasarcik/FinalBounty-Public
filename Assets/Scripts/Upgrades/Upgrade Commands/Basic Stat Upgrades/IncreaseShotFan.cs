using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseShotFan : IUpgrade
    {
        public string name{get;} = "Shot Fan Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.projectileFanCount += 2;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.projectileFanCount -= 2;
        }
    }
}
