using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class AddShot : IUpgrade
    {
        public string name{get;} = "Projectile Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.shotNumberOutputted += 1;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.shotNumberOutputted -= 1;
        }
    }
}
