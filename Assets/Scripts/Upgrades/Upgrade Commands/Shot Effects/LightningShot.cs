using PlayerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Upgrades
{
	public class LightningShot : IUpgrade
	{
		public string name{get;} = "Lightning Shot";
		public GameObject upgradeObj { get; set; }

		public void Take(IPlayer player)
		{
			player.lightningShot = true;
			Debug.Log("hello");
		}

		public void Remove(IPlayer player)
		{
			player.lightningShot = false;
		}
	}
}
