using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=PoglGJoDcZg&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=46
/// <summary>
/// Scriptable Object that holds the base stats for an enemy
/// </summary>
/// 

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")] //lets us right click->create in the project pane
public class EnemyScriptableObject : ScriptableObject
{
    //Enemy Stats
    public int Health = 100;
    public float AttackDelay = 1f;
    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public bool IsRanged = false;

    //Navmesh Config
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8f;
    public float AngularSpeed = 120f;

    //-1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;
    
}
