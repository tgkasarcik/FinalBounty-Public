using PlanetGen;
using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : IEnemy
{


    private EnemyMovement2 MovementScript;

    private EnemyMovement2 MoveScript;

    IPlayer Player;
    IGunType MyWeapon;

    bool Wandering = false;
    bool Chasing = false;
    List<int> CellIndicies = new List<int>();

    [SerializeField] public float activationDistance;
    [SerializeField] public float secondsBetweenShots;
    [SerializeField] public float wanderSpeed;


    private readonly int MIN_CELL_DIST = 20;


    public override void Awake()
    {
        base.Awake();
        Health = 10f;
        moneyOnDrop = 7;
    }

    public void SetStartCell(Cell cell)
    {
        this.StartCell = cell;
    }

    public void SetStartPlanet(GenPlanet planet)
    {
        this.MyPlanet = planet;
    }

    void Start()
    {
        this.MovementScript = gameObject.GetComponent<EnemyMovement2>();
        this.MoveScript = gameObject.GetComponent<EnemyMovement2>();
        this.MoveScript.movementSpeed = wanderSpeed;
        Health = 10;

        this.fireDamageAmount = 0f;

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

        float waitTimeWander = 0.25f;
        IEnumerator facePlayerCoroutine = FacePlayer();
        bool isCoroutineRunning = false;

        yield return new WaitForSeconds(1);
        //Debug.Log("Starting movement");

        while (gameObject.activeSelf)
        {

            bool shouldWait = false;

            List<Cell> path = GetPath();

            int num = 0;
            float elapsedTime = 0;


            if (path.Count > 0)
            {

                while (num < path.Count)
                {

                    float dist = DistanceFromPlayer();
                    if (dist < activationDistance)
                    {


                        if (isCoroutineRunning == false)
                        {

                            isCoroutineRunning = true;
                            StartCoroutine(facePlayerCoroutine);
                        }

                        MyWeapon.Shoot();

                        yield return new WaitForSeconds(secondsBetweenShots);

                    }
                    else
                    {

                        if (isCoroutineRunning == true)
                        {

                            isCoroutineRunning = false;
                            StopCoroutine(facePlayerCoroutine);
                        }
                        yield return new WaitForSeconds(waitTimeWander);
                    }

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
