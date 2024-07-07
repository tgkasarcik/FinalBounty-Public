using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    //very high damage and shot size but low fire rate
    public class Polyphemus : IUpgrade
    {
        public string name{get;} = "Polyphemus";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.fireRatePercent -= .7f;
            player.damageOut += 6;
            player.damageOutPercent += .5f;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.fireRatePercent += .7f;
            player.damageOut -= 6;
            player.damageOutPercent -= .5f;
        }
    }
}
