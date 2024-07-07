using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=5uO0dXYbL-s&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=46
//https://www.youtube.com/watch?v=Aee01YxQIsw&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=45
public class Enemy : PoolableObject, IDamageable
{
    public AttackRadius AttackRadius;
    public EnemyMovement Movement;
    public NavMeshAgent Agent;
    public EnemyScriptableObject EnemyScriptableObject;
    public int Health = 100;


    private Coroutine LookCoroutine;
    private const string Attack_Trigger = "Attack";

    private void Awake()
    {
        AttackRadius.OnAttack += OnAttack;
    }

    private void OnAttack(IDamageable target)
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }









    //Using onEnable instead of start because the enemey will be disabled and re-enabled multiple times, 
    //start would only let us set them up once but we want to set them up every time we put them back into the world
    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Agent.enabled = false; 
    }

    //Virtual allows us to sub-class enemy types in the future
    public virtual void SetupAgentFromConfiguration()
    {
        Agent.acceleration = EnemyScriptableObject.Acceleration;
        Agent.angularSpeed = EnemyScriptableObject.AngularSpeed;
        Agent.areaMask = EnemyScriptableObject.AreaMask;
        Agent.avoidancePriority = EnemyScriptableObject.AvoidancePriority;
        Agent.baseOffset = EnemyScriptableObject.BaseOffset;
        Agent.height = EnemyScriptableObject.Height;
        Agent.obstacleAvoidanceType = EnemyScriptableObject.ObstacleAvoidanceType;
        Agent.radius = EnemyScriptableObject.Radius;
        Agent.speed = EnemyScriptableObject.Speed;
        Agent.stoppingDistance = EnemyScriptableObject.StoppingDistance;

        Movement.UpdateRate = EnemyScriptableObject.AIUpdateInterval;
        Health = EnemyScriptableObject.Health; //health reset on respawn

        AttackRadius.Collider.radius = EnemyScriptableObject.AttackRadius;
        AttackRadius.AttackDelay = EnemyScriptableObject.AttackDelay;
        AttackRadius.Damage = EnemyScriptableObject.Damage;

    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform() { return transform; }
}
