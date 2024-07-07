using PlayerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Upgrades
{
	public class ControlledShot : IUpgrade
	{
		public string name{get;} = "Controlled Shot";
		public GameObject upgradeObj { get; set; }

		public void Take(IPlayer player)
		{
			player.controledShot = true;
		}

		public void Remove(IPlayer player)
		{
			player.controledShot = false;
		}
	}
}
