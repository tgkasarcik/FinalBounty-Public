using PlanetGen;
using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//UNUSED CLASS - WIP
public class SmallBoi : IEnemy
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
    [SerializeField] public float orbitSpeed;





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

        MyWeapon = gameObject.AddComponent<Lazer>();
        MyWeapon.Initialize(this, gameObject, MyPlanet.Planet);
		Health = 5f;




		System.Random rand = new System.Random();
 
        Wander();


    }




    public void DestroySequence() 
    {
        Destroy(gameObject);
        Instantiate(ExplosionFX, transform.position, transform.rotation);


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


    //This code desperately needs refactoring... but graduation is in a week and a half so oh well
    private IEnumerator NavigatePath()
    {

        float waitTimeWander = 0.25f;
        IEnumerator facePlayerCoroutine = FacePlayer();
        bool isCoroutineRunning = false;
        bool isTrackingPlayer = false;
        float rotationAngle = 0f;
        Vector3 initialLocalPosition = Vector3.zero;



        yield return new WaitForSeconds(1);
        //Debug.Log("Starting movement");

        while (gameObject.activeSelf)
        {

            bool shouldWait = false;

            List<Cell> path = GetPath();

            int num = 0;
            if (isTrackingPlayer)
            {

                //Vector3 adjustedLocation = gameObject.transform.position - this.Player.playerObj.transform.position; 
                //Vector3 roatedAroundAdjusted = Quaternion.Euler(10 * Time.deltaTime, 10 * Time.deltaTime, 10 * Time.deltaTime) * adjustedLocation;
                //Vector3 finalLocation = adjustedLocation + this.Player.playerObj.transform.position;
                //Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

                //Vector3 preRotationLocation = this.Player.playerObj.transform.position + transform.TransformDirection(rotation * this.Player.playerObj.transform.forward)* 10f;
                //Vector3 finalLocation = rotation * preRotationLocation;
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = finalLocation;

                //float dist = DistanceFromPlayer();
                //if (dist < 15f)
                //{



                //    Vector3 rightDirection = gameObject.transform.TransformDirection(Vector3.right).normalized;
                //    GetComponent<Rigidbody>().AddForce(rightDirection * 0.10f);


                //} else
                //{

                //    MovementScript.ChangeGoal(this.Player.playerObj.transform.position);
                //}
                
                rotationAngle += Time.deltaTime * 3;

                Vector3 offset = new Vector3(Mathf.Cos(rotationAngle) * 10f, 0f, Mathf.Sin(rotationAngle) * 10f);
                transform.localPosition = initialLocalPosition + offset;
                transform.position = this.Player.playerObj.transform.TransformPoint(transform.localPosition);




                yield return new WaitForSeconds(0);

            }
            else
            {



                while (num < path.Count && isTrackingPlayer == false)
                {
                    float dist = DistanceFromPlayer();
                    if (dist < activationDistance || isTrackingPlayer == true)
                    {

                        if (isCoroutineRunning == false)
                        {
                            isCoroutineRunning = true;
                            StartCoroutine(facePlayerCoroutine);
                        }

                        MyWeapon.Shoot();

                        isTrackingPlayer = true;
                        this.MoveScript.movementSpeed = 0;

                        initialLocalPosition = this.Player.playerObj.transform.InverseTransformPoint(transform.position);

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
                        //Wait to reach the next node in the path
                        if (AdjustedMagnitudeFromCell(path[num - 1]) < 10)
                        {
                            shouldWait = false;
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

                this.StartCell = path[num - 1]; //inner loop stops when this goes OOB
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

    public void Wander()
    {
        StartCoroutine(NavigatePath());
        this.Wandering = true;
    }

    public void Chase(IPlayer player)
    {
        //drones dont do this shit
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damageAmount)
    {
        this.Health -= damageAmount;
        if (this.Health < 0)
        {
            DestroySequence();

        }
    }
}
