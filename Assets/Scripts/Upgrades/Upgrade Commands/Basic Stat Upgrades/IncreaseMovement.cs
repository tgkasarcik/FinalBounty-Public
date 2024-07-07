using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class IncreaseMovement : IUpgrade
    {
        public string name{get;} = "Movement Up";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.movementSpeed += 5;
            player.playerMovement.movementSpeed += 5;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.movementSpeed -= 5;
            player.playerMovement.movementSpeed -= 5;
        }
    }
}
