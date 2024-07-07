using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBaseMovement : MonoBehaviour
{
    private GameObject player;
    [SerializeField] public float rotationSpeed = 10f;

    private Vector3 normal;
    private void Start()
    {
        normal = transform.up;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        Vector3 playerDir = (transform.position - player.transform.position).normalized;
        Vector3 playerOnNormal = Vector3.ProjectOnPlane(-playerDir, normal);

        Quaternion goalQ = Quaternion.LookRotation(playerOnNormal, normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, goalQ, Time.deltaTime * rotationSpeed);
    }


}
