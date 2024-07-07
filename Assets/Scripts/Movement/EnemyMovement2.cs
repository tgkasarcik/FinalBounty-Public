using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement2 : Movement
{
    private Rigidbody rbEnemy;
    [SerializeField] public GameObject goalObject;

    Vector3 goal;
    Vector3 goalDir;
    GameObject planet;
    
    void Start()
    {
        goalObject = GameObject.FindWithTag("Player");
        rbEnemy = gameObject.GetComponent<Rigidbody>();
        //goal = goalObject.transform.position;

        GameObject test = GameObject.FindWithTag("Planet");

        gameObject.GetComponent<GravityBody>().SetPlanetObj(test);

        planet = gameObject.GetComponent<GravityBody>().planetObj;
    }

    protected override void FixedUpdate()
    {

        if (orbiting)
        {
            goalDir = Vector3.ProjectOnPlane(goal - transform.position, planet.transform.position - transform.position).normalized;
        }
        else
        {
            goalDir = (goal - transform.position).normalized;
        }
        RotateTowards(goalDir);
        MoveTowards(goalDir);


        Debug.DrawRay(transform.position, (goal - transform.position), Color.green, 10f);
        Debug.DrawRay(transform.position, Vector3.ProjectOnPlane(goal - transform.position, planet.transform.position - transform.position), Color.red, 10f);
    }

    //Moves the Object towards a direction
    public void MoveTowards (Vector3 goalDir)
    {
        moveWithForces(goalDir.magnitude, goalDir, Vector3.Dot(rbEnemy.velocity, goalDir), rbEnemy);
    }

    //Faces the object to face a certain direction
    public void RotateTowards (Vector3 goalDir)
    {
        Quaternion lookAt = Quaternion.FromToRotation(transform.forward, (goalDir - transform.position).normalized);
        transform.rotation = lookAt * transform.rotation;
    }

    //Changes the goal the object is approaching
    public void ChangeGoal (Vector3 goal)
    {
        rbEnemy.velocity = new Vector3(0, 0, 0); //reset velocity every time we have a new goal to go towards... prevents unhinged acceleration

        this.goal = goal;
    }

    public void StopMoving()
    {
        rbEnemy.velocity = new Vector3(0, 0, 0); //reset velocity every time we have a new goal to go towards... prevents unhinged acceleration

    }
}
