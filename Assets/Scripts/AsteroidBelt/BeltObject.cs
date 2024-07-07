using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code from: https://github.com/joeythelantern/AsteroidBeltExample

public class BeltObject : MonoBehaviour
{
    [SerializeField]
    private float orbitSpeed;
    [SerializeField]
    private GameObject parent;
    [SerializeField]
    private bool rotationClockwise;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private Vector3 rotationDirection;
    private float distanceToRotation;
    private Rigidbody rb;

    public void SetupBeltObject(float _speed, float _rotationSpeed, GameObject _parent, bool _rotateClockwise)
    {
        orbitSpeed = _speed;
        rotationSpeed = _rotationSpeed;
        parent = _parent;
        rotationClockwise = _rotateClockwise;
        rotationDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        distanceToRotation = (transform.position - _parent.transform.position).magnitude;

        gameObject.layer = LayerMask.NameToLayer("Asteroids");
    }

    private void Update()
    {
        if (rotationClockwise)
        {
            transform.RotateAround(parent.transform.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(parent.transform.position, -Vector3.up, orbitSpeed * Time.deltaTime);
        }

        //Makes the y position always 0
        if (transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }


        transform.Rotate(rotationDirection, rotationSpeed * Time.deltaTime);
    }
}