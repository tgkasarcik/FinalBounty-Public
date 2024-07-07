using PlayerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Upgrades
{
	internal class IncreasePlayerSize : IUpgrade
	{
		public string name{get;} = "Player Size Up";
		public GameObject upgradeObj { get; set; }
		//takes upgrade (applies it)
		public void Take(IPlayer player)
		{
			player.playerSize += .1f;
		}

		//removes upgrade (unapplies it)
		public void Remove(IPlayer player)
		{
			if (player.playerSize > .1)
			{
				player.playerSize -= .1f;
			}
			else
			{
				player.playerSize = player.playerSize * .9f;
			}
		}
	}
}
