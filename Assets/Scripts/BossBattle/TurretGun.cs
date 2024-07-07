using PlanetGen;
using PlayerObject;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretGun : IEnemy
{
    private GameObject playerObj;
    [SerializeField] public float rotSpeed = 10f;
    [SerializeField] public float attackingRange = 1000f;

    [SerializeField] public float lazerSpeed = 50f;
    [SerializeField] public float lazerFireRate = 10f;
    [SerializeField] public float lazerFireRatePercent = 1f;

    [SerializeField] public float rocketSpeed = 50f;
    [SerializeField] public float rocketFireRate = 1f;
    [SerializeField] public float rocketFireRatePercent = 1f;

    private IGunType gunType;

    [SerializeField] public bool lazer = true;
    [SerializeField] public bool rocket = false;

    private Transform[] turretTransforms;

    private Vector3 right;
    private Vector3 normal;

    bool Wandering = false;
    bool Chasing = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        right = transform.right;
        normal = transform.up;
        turretTransforms = gameObject.GetComponentsInChildren<Transform>();

        int level = PlayerLevelManager.GetCurrentLevel() + 1;

        //Adds the weapon to each gun on the turret
        if (lazer)
        {
            foreach (Transform t in turretTransforms)
            {
                if(t != transform)
                {
                    IGunType lazer = t.AddComponent<Lazer>();
                    lazer.Initialize(this, gameObject);

                    lazer.projectileSpeed = lazerSpeed;
                    lazer.fireRate = lazerFireRate * level;
                    lazer.fireRatePercent = lazerFireRatePercent;
                    lazer.ammo = int.MaxValue;
                    lazer.bossFightBehavior = true;
                    lazer.player = PlayerManager.players[0];
                }
            }
        }

        else
        {
            foreach(Transform t in turretTransforms)
            {
                if( t != transform)
                {
                    IGunType rocket = t.AddComponent<Rocket>();
                    rocket.Initialize(this, gameObject);

                    rocket.projectileSpeed = rocketSpeed;
                    rocket.fireRate = rocketFireRate * level;
                    rocket.fireRatePercent = rocketFireRatePercent;
                    rocket.ammo = int.MaxValue;
                    rocket.bossFightBehavior = true;
                    rocket.player = PlayerManager.players[0];
                }
                
            }
            
        }
        
    }

    //disables collision impact damage
    public override void OnCollisionEnter(Collision collision)
    {

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Makes sure the turret knows where the player is
        if(playerObj == null)
        {
            playerObj = GameObject.FindWithTag("Player");
        }
        else
        {
            turretTransforms = gameObject.GetComponentsInChildren<Transform>();
            float distanceToPlayer = (transform.position - playerObj.transform.position).magnitude;
            if(distanceToPlayer <= attackingRange)
            {
                Track(playerObj);
            }
        }
    }

    public override void Wander()
    {

    }

    public void Track(GameObject playerObj)
    {
        //Predictes where the player is going to move to 
        Vector3 playerVelocity = playerObj.GetComponent<Rigidbody>().velocity;
        Vector3 predictedPlayerPosition = playerObj.transform.position + playerVelocity / 2;

        Vector3 playerDir = -(transform.position - predictedPlayerPosition).normalized;

        transform.forward = Vector3.Lerp(transform.forward, playerDir, Time.deltaTime * rotSpeed);

        //Only shoots if the the turret can see the player and the player is in the boss room
        if(CanSeeObj(transform, playerObj.transform, "Player"))
        {
            if((PlanetRoomManager.currentPlanet != null && PlanetRoomManager.currentPlanet.Planet != null &&PlanetRoomManager.currentPlanet.Planet.CompareTag("Boss")))
            {
                foreach (Transform turret in turretTransforms)
                {
                    //Get components in children also returns the base component
                    IGunType weapon = turret.GetComponent<IGunType>();
                    if (weapon != null)
                    {
                        weapon.AiFire(transform.forward, turret.position, turret);
                    }
                }
            }
        }
        
    }

    //Uses a raycast to see if one transform can see another
    public bool CanSeeObj(Transform objOne, Transform objTwo, string tagToCheck)
    {
        bool canSeeObj = false;

        RaycastHit hit;
        if (Physics.Raycast(objOne.position, objTwo.position - objOne.position, out hit, float.MaxValue))
        {
            if (hit.transform.tag.Equals(tagToCheck))
            {
                canSeeObj = true;
            }
        }

        return canSeeObj;
    }

    public override void Chase(IPlayer player)
    {
        
    }

	public override void SetStartCell(Cell cell)
	{
		throw new System.NotImplementedException();
	}

	public override void SetStartPlanet(GenPlanet planet)
	{
		throw new System.NotImplementedException();
	}
    public override void DestroySequence()
    {
        Destroy(this.gameObject);
    }

}
