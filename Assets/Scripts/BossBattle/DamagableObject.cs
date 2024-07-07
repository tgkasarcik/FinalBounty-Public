using PlanetGen;
using PlayerObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : IEnemy
{
    public override void Chase(IPlayer player)
    {
        throw new System.NotImplementedException();
    }

    public override void DestroySequence()
    {
        Destroy(this.gameObject);
    }

	public override void SetStartCell(Cell cell)
	{
		throw new System.NotImplementedException();
	}

	public override void SetStartPlanet(GenPlanet planet)
	{
		throw new System.NotImplementedException();
	}

	public override void OnCollisionEnter(Collision collision)
	{
		
	}

    public override void Wander()
    {
        throw new System.NotImplementedException();
    }

}
