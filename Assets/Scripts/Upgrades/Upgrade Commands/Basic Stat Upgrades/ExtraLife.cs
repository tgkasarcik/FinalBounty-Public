using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class ExtraLife : IUpgrade
    {
        public string name{get;} = "Extra Life";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.lives += 1;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            if(player.lives > 1)
            {
                player.lives -= 1;
            }
        }
    }
}
