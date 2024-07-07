using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class DecreasePlayerSize : IUpgrade
    {
        public string name{get;} = "Player Size Down";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            if(player.playerSize > .1)
            {
                player.playerSize -= .1f;
            }
            else
            {
                player.playerSize = player.playerSize* .9f;
            }
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.playerSize += .1f;
        }
    }
}
