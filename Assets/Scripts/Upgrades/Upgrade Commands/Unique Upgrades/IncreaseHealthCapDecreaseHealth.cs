using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseHealthCapDecreaseHealth : IUpgrade
    {
        public string name{get;} = "Health Swap";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.Health += 50;
            player.currentHealth -= 50;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.Health -= 50;
        }
    }
}
