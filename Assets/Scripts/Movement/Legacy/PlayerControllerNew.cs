using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.Rendering;

//This Movement IS movement based and NOT event based
public class PlayerControllerNew : MonoBehaviour
{
    [SerializeField]
    public float Velocity = 0;
    public float turningRate = 0;

    public event EventHandler<Collider> PowerUpCollision;
    private InputActions inputActions;
    private InputAction movement;
    private Rigidbody rb;
    private bool moveCanceled;
    private Quaternion originalRot = Quaternion.identity;

    public float yaw;
    public float roll;

    public float maxRoll = 45f;
    public float yawSpeed = 100f;
    public float rollSpeed = 100f;

    public float yawAcceleration = 0f;

    private void OnEnable()
    {
        moveCanceled = false;
        originalRot = transform.localRotation;
        transform.rotation = Quaternion.identity;
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        movement = inputActions.Player.Move;
        movement.Enable();
    }

    private void FixedUpdate()
    {

        var inputMovement = movement.ReadValue<Vector2>();
        var worldMovement = new Vector3(inputMovement.x, 0.0f, inputMovement.y);
       
        //If Player does a moving input
        if(inputMovement.x == 0 && inputMovement.y == 0)
        {
            Vector3 currentDirection = rb.velocity;
            worldMovement = -currentDirection / 4f;
            yawAcceleration = 0f;
        }
        else
        {
            float yawGoal = Mathf.Rad2Deg * Mathf.Atan2(-inputMovement.y, inputMovement.x);
            float lastYaw = yaw;
            yaw = Mathf.MoveTowardsAngle(yaw, yawGoal, yawSpeed * Time.fixedDeltaTime);
           yawAcceleration = lastYaw - yaw;
        }

        //Angle of the velocity vector against the ships right vector
        Vector3 velocity = rb.velocity.normalized;
        float angle = Vector3.SignedAngle(velocity, transform.right.normalized, Vector3.up);
        float radianAngle = Mathf.Deg2Rad * angle;

        //Roll can either use the yaw acceleration or radian angle
        float rollGoal = Mathf.Clamp(yawAcceleration, -1f, 1f) * maxRoll;

        Debug.DrawRay(transform.position, transform.right, Color.yellow, 3f);
        Debug.DrawRay(transform.position, velocity, Color.red, 3f);
        Debug.Log($"Velocity: {rb.velocity.normalized}");

        roll = Mathf.MoveTowardsAngle(roll, rollGoal, rollSpeed * Time.fixedDeltaTime);

        //Final rotation finalized and force added
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(roll, yaw, 0f), turningRate * Time.fixedDeltaTime);
        rb.AddForce(worldMovement * Velocity);

       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            OnPowerUpCollision(other);
        }
    }

    protected virtual void OnPowerUpCollision(Collider powerUpCollider)
    {
        PowerUpCollision?.Invoke(this, powerUpCollider);
    }

    private void OnDisable()
    {
        movement.Disable();
    }

    public void OnMoveCancel(bool moveCanceled)
    {
        this.moveCanceled = moveCanceled;
    }
}
