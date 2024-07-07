using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class BeamWeapon : IUpgrade
    {
        public string name{get;} = "Beam Weapon";
        public GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        public void Take(IPlayer player){
            player.haveBeam = true;
        }

        //removes upgrade (unapplies it)
        public void Remove(IPlayer player)
        {
            player.haveBeam = false;
        }
    }
}
