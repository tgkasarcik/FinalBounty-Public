using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=Aee01YxQIsw&list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC&index=45
public class Player : MonoBehaviour, IDamageable
{

    [SerializeField] private AttackRadius AttackRadius;
    private Coroutine LookCoroutine;

    [SerializeField] private int Health = 300;



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

    //Assuming that this script is attached to the root level player object
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0 )
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

}
