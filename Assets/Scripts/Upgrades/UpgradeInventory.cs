using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

namespace Upgrades
{
    public class UpgradeInventory : IUpgradeInventory
    {
        public List<IUpgrade> upgrades {get;}

        private IPlayer player;

        private HUDInteractions hud;

        public UpgradeInventory(IPlayer player)
        {
            upgrades = new List<IUpgrade>();
            this.player = player;
        }

        //adds upgrade to list
        public void AddUpgrade(IUpgrade upgrade)
        {
            upgrades.Add(upgrade);
            upgrade.Take(this.player);
            this.player.projectiles.SetAttributes();
            GameObject hudScripts = GameObject.Find("HUDScripts");
            hud = hudScripts.GetComponent<HUDInteractions>();
            hud.AddUpgradeToList(upgrade.name);
        }

        //removes upgrade from list of upgrades from index
        public void RemoveUpgrade(int index)
        {
            upgrades[index].Remove(this.player);
            GameObject hudScripts = GameObject.Find("HUDScripts");
            hud = hudScripts.GetComponent<HUDInteractions>();
            hud.RemoveUpgradeFromList(upgrades[index].name);
            upgrades.RemoveAt(index);
            this.player.projectiles.SetAttributes();
        }

        //removes upgrade from list of upgrades by object type
        public void RemoveUpgrade(IUpgrade upgrade)
        {
            upgrades.Remove(upgrade);
            upgrade.Remove(this.player);
            this.player.projectiles.SetAttributes();
        }
    }
}
