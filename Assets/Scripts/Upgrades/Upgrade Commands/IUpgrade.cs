using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public interface IUpgrade
    {
        string name {get;}
        GameObject upgradeObj {get;set;}
        //takes upgrade (applies it)
        void Take(IPlayer player);

        //removes upgrade (unapplies it)
        void Remove(IPlayer player);
    }
}
