using PlayerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Upgrades
{
	public class RefillRocketAmmo : IUpgrade
	{
		public string name{get;} = "Refill Rocket Ammo";
		public GameObject upgradeObj { get; set; }
		//takes upgrade (applies it)
		public void Take(IPlayer player)
		{
			player.rocketAmmo = player.maxRocketAmmo;
		}

		//removes upgrade (unapplies it)
		public void Remove(IPlayer player)
		{
			// do nothing, since it wouldn't make sense for the player to lose their rockets for taking away the upgrade
		}
	}
}
