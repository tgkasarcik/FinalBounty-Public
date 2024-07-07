using PlanetGen;
using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Ram : IEnemy
{


    private EnemyMovement2 MovementScript;

    private EnemyMovement2 MoveScript;

    IPlayer Player;
    IGunType MyWeapon;

    bool Wandering = false;
    bool Chasing = false;
    List<int> CellIndicies = new List<int>();

    [SerializeField] public float wanderingMovespeed;
    [SerializeField] public float attackingAddedInitialSpeedBoost;
    [SerializeField] public float attackingAccelerationMultiplier;
    [SerializeField] public float activationDistance;

    private readonly int MIN_CELL_DIST = 20;

    public void SetStartCell(Cell cell)
    {
        this.StartCell = cell;
    }

    public void SetStartPlanet(GenPlanet planet)
    {
        this.MyPlanet = planet;
    }
    public override void Awake()
    {
        base.Awake();
        Health = 5f;
        moneyOnDrop = 15;
    }


    void Start()
    {
        this.MovementScript = gameObject.GetComponent<EnemyMovement2>();
        this.MoveScript = gameObject.GetComponent<EnemyMovement2>();
        this.MoveScript.movementSpeed = wanderingMovespeed;

        MyWeapon = gameObject.AddComponent<Lazer>();
        MyWeapon.Initialize(this, gameObject, MyPlanet.Planet);



        System.Random rand = new System.Random();
 
        Wander();


    }


    private float AdjustedMagnitudeFromCell(Cell targetCell)
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 destPos = targetCell.getLocation();
        Vector3 adjusted = pos - destPos;
        float magnitude = Vector3.Magnitude(adjusted);

        return magnitude;
    }

    //only works for one player in the game (can be expanded later) !!!
    private float DistanceFromPlayer()
    {
        //This is inefficient
        List<IPlayer> playerList = PlayerManager.players;
        foreach (IPlayer player in playerList)
        {
            Vector3 playerPos = player.playerObj.transform.position;
            Vector3 myPos = gameObject.transform.position;
            Vector3 adjusted = myPos - playerPos;
            float magnitude = Vector3.Magnitude(adjusted);
            //Debug.Log("Magnitude to player: " + magnitude);
            this.Player = player;
            return magnitude;
        }

        return 999999f;

    }

    private IEnumerator FacePlayer()
    {
        while (true)
        {
            this.MovementScript.RotateTowards(this.Player.playerObj.transform.position);
            yield return new WaitForSeconds(0); //0 means wait 1 frame
        }
    }

    private IEnumerator NavigatePath()
    {

        float waitTime = 0f; //frame  
        float waitTimeWander = 0.25f;
        IEnumerator facePlayerCoroutine = FacePlayer();
        bool isCoroutineRunning = false;

        yield return new WaitForSeconds(1);
        //Debug.Log("Starting movement");

        while (gameObject.activeSelf)
        {

            bool shouldWait = false;
            bool chargeAttempted = false;
            Vector3 chargeTarget = Vector3.zero; 

            List<Cell> path = GetPath();

            int num = 0;
            float elapsedTime = 0;
            float elapsedTimeBoom = 0;


            if (path.Count > 0)
            {

                while (num < path.Count)
                {
                    float dist = DistanceFromPlayer();
                    if (dist < activationDistance && chargeAttempted == false)
                    {
                        chargeAttempted = true;
                        chargeTarget = this.Player.playerObj.transform.position;
                        MovementScript.ChangeGoal(chargeTarget);
                        this.MoveScript.movementSpeed += attackingAddedInitialSpeedBoost; //why am i writing to this every frame



                        yield return new WaitForSeconds(waitTime);

                    }
                    else
                    {
                        //routine wandering
                        if (chargeAttempted == false)
                        {
                            if (isCoroutineRunning == true)
                            {
                                isCoroutineRunning = false;
                                StopCoroutine(facePlayerCoroutine);
                            }
                            yield return new WaitForSeconds(waitTimeWander);
                        }
                        //we were charging, but for some reason we stopped (without being destroyed)
                        else
                        {
                            if (chargeTarget != Vector3.zero)
                            {
                                elapsedTimeBoom += Time.deltaTime;
                                //have we reached our destination
                                Vector3 pos = gameObject.transform.position;
                                Vector3 destPos = chargeTarget;
                                Vector3 adjusted = pos - destPos;
                                float magnitude = Vector3.Magnitude(adjusted);
                                this.MoveScript.movementSpeed += attackingAccelerationMultiplier * Time.deltaTime;


                                if (magnitude <= 3 || elapsedTimeBoom > 1f)
                                {
                                    //charge missed, bye bye
                                    DestroySequence();
                                }


                                yield return new WaitForSeconds(waitTime);
                            }
                            else
                            {
                                Debug.LogWarning("Error charging. Unable to get player position?");
                                break;
                            }

                        }



                    }


                    if (chargeAttempted == false)
                    {

                        if (true == shouldWait)
                        {
                            elapsedTime += Time.deltaTime;
                            //Wait to reach the next node in the path
                            if ((AdjustedMagnitudeFromCell(path[num - 1]) < MIN_CELL_DIST) || elapsedTime > 3f)
                            {
                                shouldWait = false;
                                elapsedTime = 0;
                            }

                        }
                        else
                        {


                            Vector3 location = path[num].getLocation();
                            num++;
                            MovementScript.ChangeGoal(location);


                            shouldWait = true;

                        }
                    }


                }

                //This may not be true if the charge failed, but it hopefully wont matter :)
                int indx = num - 1;
                if (indx < 0 || indx > path.Count - 1)
                {
                    Debug.LogWarning("Pathing guard triggered, n-1: " + indx + " cnt: " + path.Count);
                    indx = 0;
                }

                this.StartCell = path[indx]; //inner loop stops when this goes OOB
            }

        }

    }

    private List<Cell> GetPath()
    {

        System.Random rand = new System.Random();
        Searcher searcher = new Searcher();



        Cell[] cells = this.MyPlanet.Grid.GetCells();
        int randNum = rand.Next(cells.Length - 1);
        Cell endCell = cells[randNum];


        return searcher.RefreshPath(this.StartCell, endCell);
    }

    public override void Wander()
    {
        StartCoroutine(NavigatePath());
        this.Wandering = true;
    }

    public override void Chase(IPlayer player)
    {
        //drones dont do this shit
        throw new System.NotImplementedException();
    }
}
