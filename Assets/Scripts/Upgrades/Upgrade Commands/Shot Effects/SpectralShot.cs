using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class SpectralShot : IUpgrade
    {
        public string name{get;} = "Spectral Shot";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.spectralShot = true;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.spectralShot = false;
        }
    }
}
