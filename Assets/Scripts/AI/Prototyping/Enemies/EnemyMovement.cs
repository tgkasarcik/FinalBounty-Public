using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//https://www.youtube.com/watch?v=5uO0dXYbL-s&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=46
public class EnemyMovement : MonoBehaviour
{

    public Transform Player;
    public float UpdateRate = 0.1f;
    public float ChaseDistance = 15f;
    public NavMeshTriangulation Triangulation;

    private NavMeshAgent Agent;

    private Coroutine MovementStateCoroutine;
    private MovementState CurrentState;
    private Vector3 Destination;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }


    public void StartMovement()
    {
        if (MovementStateCoroutine == null)
        {
            CurrentState = MovementState.Wander;
            Destination = GetRandomWanderDestination();
            MovementStateCoroutine = StartCoroutine(StateCheck());
        }
    }

    //TODO: pull logic out of this method
    private IEnumerator StateCheck()
    {
        while (enabled) {
            Debug.Log("State Check");
            float distance = Vector3.Distance(Agent.transform.position, Player.transform.position);

            //Check if we should start chasing
            if (CurrentState == MovementState.Wander && distance <= ChaseDistance)
            {
                CurrentState = MovementState.Chase;
                Destination = Player.transform.position;
            }
            //Enemies that are already chasing should track the player over a larger distance
            else if (CurrentState == MovementState.Chase && distance <= (ChaseDistance * 1.5))
            {
                Destination = Player.transform.position;
            }
            //We're wandering now!
            else
            {
                if (CurrentState == MovementState.Chase)
                {
                    CurrentState = MovementState.Wander;
                    Destination = GetRandomWanderDestination();
                }
                else
                {
                    //Close enough, find a new waypoint
                    //TODO: don't account for Y at all, or account for agent height
                    if (Vector3.Distance(Agent.transform.position, Destination) < 5)
                    {
                        Destination = GetRandomWanderDestination();
                    }
                }
            }

            Agent.SetDestination(Destination);

            yield return new WaitForSeconds(UpdateRate);
        } 
    }

    private Vector3 GetRandomWanderDestination()
    {
        int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

        NavMeshHit Hit;
        if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, 1)) //1 for walkable areas
        {
            Debug.Log("Going to" + Hit.position);
            return Hit.position;
        }

        Debug.LogError("Unable to generate a wandering waypoint");

        return new Vector3(0, 0, 0);
    }

    private enum MovementState
    {
        Chase,
        Wander
    }



}
