using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;
using System;
using PlayerObject;

namespace PlayerObject
{
	public class PlayerShip : IPlayer
	{
		public static event EventHandler GameOverEvent;

		public GameObject prefab { get; set; }

		//gun ammo
		public int rocketAmmo { get; set; }
        public int maxRocketAmmo {get;set;}

        public int beamAmmo {get;set;}
        public int maxBeamAmmo {get;set;}

		public bool haveBeam {get;set;}

		//projectile data
		public float damageOut { get; set; }
		public float damageOutPercent { get; set; }
		public int shotNumberOutputted { get; set; }
		public int projectileFanCount { get; set; }
		public float shootFOV { get; set; }
		public float projectileTime { get; set; }
		public float projectileSpeed { get; set; }
		public float projectileSize { get; set; }
		public float fireRate { get; set; }
		public float fireRatePercent { get; set; }

		//different shot modifiers
		public bool flameShot { get; set; }
		public float flameDamage { get; set; }

		public bool homingShot { get; set; }

		public bool controledShot { get; set; }

		public bool gravityShot { get; set; }
		public float sensingDistance { get; set; }

		public bool pierceShot { get; set; }
		public bool lightningShot { get; set; }
		public bool spectralShot { get; set; }

		//movement data
		public float movementSpeed { get; set; }
		public float spaceMovementSpeed { get; set; }
		public bool invunerable { get; set; }
		public bool rolling { get; set; }

		//player data
		public string name { get; set; }
		public int lives { get; set; }
		public float Health { get; set; }
		public float currentHealth { get; set; }
		public float playerSize { get; set; }
        public bool onFire {get;set;}


		//misc
		public MovementWithAbilities playerMovement { get; set; }

		public IUpgradeInventory inventory { get; set; }

        public bool bossBattle {get;set;}

		public GameObject playerObj { get; set; }

		public PlayerProjectiles projectiles { get; set; }
		public bool onPlanet { get; set; }

		public List<GameObject> projectilePrefabs { get; set; }

		public RedDamagePlayer RedDamage;


		public PlayerShip(List<GameObject> projectilePrefabs)
		{
			inventory = new UpgradeInventory(this);
			this.projectilePrefabs = projectilePrefabs;
			this.name = "Player";
			Health = 100;
			currentHealth = 100;
			damageOut = 1f;
			projectileSize = 1;
			shotNumberOutputted = 1;
			projectileFanCount = 1;
			shootFOV = 170f;
			projectileSpeed = 15f;
			fireRate = 4;
			fireRatePercent = 1f;
			movementSpeed = 10f;
			spaceMovementSpeed = movementSpeed * 2;
			projectileTime = 5f;
			sensingDistance = 5f;
			lives = 1;
			homingShot = false;
			gravityShot = false;
			lightningShot = false;
			flameDamage = .5f;
			playerSize = 1f;
			flameShot = false;
			pierceShot = false;
			spectralShot = false;
			controledShot = false;
			haveBeam = false;

            rocketAmmo = 10;
            maxRocketAmmo = 10;

            beamAmmo = 0;
            maxBeamAmmo = 100;

            projectiles = new PlayerProjectiles(this);
        }
        public GameObject CreatePlayerObj(Vector3 position, Quaternion rotation)
        {
            playerObj = GameObject.Instantiate(prefab, position, rotation);
            //playerObj.layer = LayerMask.GetMask("Player");

			InitPlayerAttributes();
        

			return playerObj;
		}

        public void InitPlayerAttributes()
        {
            //init projectiles
            List<Type> guns = new List<Type>();
            guns.Add(typeof(Lazer));
            guns.Add(typeof(Rocket));
			if(this.haveBeam)
			{
            	guns.Add(typeof(Beam));
			}
            projectiles.guns = guns;
            projectiles.AddGunsToPlayer(playerObj);

			//init movement
			playerMovement = playerObj.GetComponent<MovementWithAbilities>();
			playerMovement.Initialize(spaceMovementSpeed);

			playerObj.transform.localScale *= playerSize;

			//init cycle weapon script
			playerObj.AddComponent<CycleWeaponHandler>();

			//red glow from damage script 
			RedDamage = playerObj.AddComponent<RedDamagePlayer>();
		}

        //sets player projectiles to have gravity
        public void SetOnPlanet(bool result)
        {
            onPlanet = result;

            //if the player is being set to be on a planet, get planet
            //to modify projectile gravity
            if(result)
            {
                //find planet by tag
                GameObject planetObj = GameObject.FindWithTag("Planet");
                if(planetObj == null)
                {
                   planetObj = GameObject.FindWithTag("Sun"); 
                    if(planetObj == null)
                    {
                        planetObj = GameObject.FindWithTag("Boss");
                    }
                }
                //set gravity for projectiles
                projectiles.SetPlanetObjects(planetObj);

				playerMovement.movementSpeed = movementSpeed;
            }
            else
            {
                projectiles.SetUseGravity(false);

				playerMovement.movementSpeed = spaceMovementSpeed;
            }

        }

		public void TakeDamage(float damageToTake)
		{
			if (currentHealth - damageToTake < 0)
			{
				lives--;
				if (lives <= 0)
				{
					// game over
					OnGameOverEvent();
				}
				else
				{
					// New life with max health
					currentHealth = Health;
				}
			} 
			else
			{
				currentHealth -= damageToTake;
			}
			if(RedDamage != null)
			{
				RedDamage.TurnRed();
			}
		}

		protected virtual void OnGameOverEvent()
		{
			GameOverEvent?.Invoke(this, new EventArgs());
		}
	}
}
