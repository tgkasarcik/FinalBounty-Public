using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrades
{
    public interface IUpgradeInventory
    {
        public List<IUpgrade> upgrades {get;}

        //adds upgrade to list
        void AddUpgrade(IUpgrade upgrade);

        //removes upgrade from list of upgrades from index
        void RemoveUpgrade(int index);

        //removes upgrade from list of upgrades by object type
        void RemoveUpgrade(IUpgrade upgrade);
    }
}
