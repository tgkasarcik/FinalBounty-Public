using PlayerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Upgrades
{
	internal class ReplenishPlayerHealth : IUpgrade
	{
		public string name{get;} = "Replenish Health";
		public GameObject upgradeObj { get; set; }
		//takes upgrade (applies it)
		public void Take(IPlayer player)
		{
			player.currentHealth = player.Health;
		}

		//removes upgrade (unapplies it)
		public void Remove(IPlayer player)
		{
			// do nothing, since it wouldn't make sense for player to lose health for taking away upgrade
		}
	}
}
