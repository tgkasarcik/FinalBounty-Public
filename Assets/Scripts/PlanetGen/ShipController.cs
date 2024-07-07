using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.Windows;
using UnityEngine.SocialPlatforms;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class ShipController : MonoBehaviour
{

    private float movementX;
    private float movementZ;

    private float roll;
    private float yaw;
    [SerializeField] public float yawSpeed = 100f;
    [SerializeField] public float maxRoll = 45f;
    [SerializeField] public float rollSpeed = 100f;

    [SerializeField] public float highSpeed = 10f;
    [SerializeField] public float normalSpeed = 5f;

    private float lookX;
    private float lookZ;

    private float deltaX = 0f;
    private float deltaZ = 0f;

    private float yawAcceleration = 0f;

    private float movementSpeed;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    private Vector3 inputVelocity;

    [SerializeField] public float smoothTime = .2f;
    [SerializeField] public float topSpeed = 1.0f;

    private float zeroFloat = 0.0f;
    private Rigidbody rb;

    public float acceleration = 13;
    public float decceleration = 16;
    public float velPower = 0.96f;

    [SerializeField] public bool physics;

    [SerializeField] public float Velocity = 0;
    private Vector3 CameraPos = new (0, 300, 0);
    private Vector3 CameraRot = new (80, 0, 0);
    public event EventHandler<Collider> PowerUpCollision;
    private InputActions inputActions;
    private InputAction movement;
    private InputAction look;
    Vector3 inputMovement;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    float sensitivity = 15f;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        movement = inputActions.Player.Move;
        movement.Enable();
        look = inputActions.Player.Look;
        look.Enable();
        movementSpeed = normalSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cell")
        {
            Debug.Log("HAHAHA: " + other.GetComponent<TriggerString>().GetTag());
        }
    }
    void Start()
    {
        SetCamera();
    }
    
    private void Update()
    {
        //Vector2 MousePos = look.ReadValue<Vector2>();
        //rb.rotation *= Quaternion.Euler(sensitivity * Time.deltaTime * new Vector3(rb.rotation.x, MousePos.x, rb.rotation.y));
       
    }
    private void FixedUpdate()
    {
        Movement();
        var localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
    private void SetCamera()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = CameraPos;
        Camera.main.transform.localEulerAngles = CameraRot;
    }
    private void Movement()
    {
        inputMovement = movement.ReadValue<Vector2>();
        float targetSpeedX = inputMovement.x * movementSpeed;
        Debug.Log(movementSpeed);
        float targetSpeedZ = inputMovement.y * movementSpeed;
        float speedDifX = targetSpeedX - rb.velocity.x;
        float speedDifZ = targetSpeedZ - rb.velocity.z;
        float accelRateX = (Mathf.Abs(targetSpeedX) > 0.01f) ? acceleration : decceleration;
        float accelRateZ = (Mathf.Abs(targetSpeedZ) > 0.01f) ? acceleration : decceleration;
        float moveX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        float moveZ = Mathf.Pow(Mathf.Abs(speedDifZ) * accelRateZ, velPower) * Mathf.Sign(speedDifZ);
        rb.AddForce(moveX * Vector3.right);
        rb.AddForce(moveZ * Vector3.forward);
    }
    

    
}
