using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseFireRate : IUpgrade
    {
        public string name{get;} = "Fire Rate Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.fireRate += 1;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.fireRate -= 1;
        }
    }
}
