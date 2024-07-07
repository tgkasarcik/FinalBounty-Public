using PlanetGen;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    protected float movementX;
    protected float movementZ;

    public Camera playerCam;

    private float lookX;
    private float lookY;

    [SerializeField] public float movementSpeed = 10f;
    [SerializeField] public float movementModifier = 2f;

    [SerializeField] public float turnSpeed = 1000f;

    [SerializeField] public bool orbiting = false;

    private bool BossBattle = false;
    [SerializeField] public bool bossBattle { get { return BossBattle; } set { BossBattle = value; gBody.ShouldRotate = !value;  } }
    

    protected Rigidbody rb;
    protected GravityBody gBody;
    protected Vector3 normal;

    public float acceleration = 13;
    public float decceleration = 16;
    public float velPower = 0.96f;

    private void OnEnable()
    {
        playerCam = Camera.main;
        rb = GetComponent<Rigidbody>();

        gBody = GetComponent<GravityBody>();
        normal = gBody.normal;
    }
    public void Initialize(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }

    protected virtual void FixedUpdate()
    {

        MovePhysicsBased();
        Rotate();

        if (!orbiting)
        {
            FixUp();
        }

        normal = gBody.normal;
    
    }

    public void Rotate()
    {
        Vector3 cameraToPlayer = playerCam.transform.position - transform.position;


        //Space in point of where the mouse is on the same plane as the transform 
        Vector3 worldSpacePoint = playerCam.ScreenToWorldPoint(new Vector3(lookX, lookY, playerCam.nearClipPlane + cameraToPlayer.magnitude));

        Vector3 goalDir = new Vector3(0,0,0);

        //Has the player look orthogonal to the planet if orbiting
        if(orbiting && !bossBattle)
        {
            goalDir = Vector3.ProjectOnPlane(worldSpacePoint - transform.position, normal).normalized;
        }
        else if(bossBattle)
        {
            gBody.ShouldRotate = false;
            RaycastHit hit;
            if (Physics.Raycast(worldSpacePoint, worldSpacePoint - playerCam.transform.position, out hit, float.MaxValue))
            {
                worldSpacePoint = hit.point;

            }
            //If the raycast doesn't hit, it looks at an approxmiate location
            else
            {
                worldSpacePoint = playerCam.ScreenToWorldPoint(new Vector3(lookX, lookY, playerCam.nearClipPlane + cameraToPlayer.magnitude + 1000));
            }

            goalDir = worldSpacePoint - transform.position;
        }
        else
        {
            goalDir = worldSpacePoint - transform.position;
        }

        Quaternion mouseLook = Quaternion.FromToRotation(transform.forward, goalDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, mouseLook * transform.rotation, Time.deltaTime * turnSpeed);
    }

    public void FixUp()
    {
        //Makes sure the up vector is facing up
        Quaternion upRotation = Quaternion.FromToRotation(transform.up, Vector3.up);
        transform.rotation = upRotation * transform.rotation;
    }

    private void OnMove(InputValue value)
    {
        Vector2 moveDir = value.Get<Vector2>();

        movementX = moveDir.x;
        movementZ = moveDir.y;
    }

    protected void MovePhysicsBased()
    { 
        moveWithForces(movementX, playerCam.transform.right, Vector3.Dot(rb.velocity, playerCam.transform.right), rb);
        moveWithForces(movementZ, playerCam.transform.up, Vector3.Dot(rb.velocity, playerCam.transform.up), rb);
    }

    //From https://youtu.be/KbtcEVCM7bw?si=EqmDL5C5wBk1otJN
    protected void moveWithForces(float moveDirection, Vector3 direction, float rbVelocity, Rigidbody rb)
    {
        float targetSpeed = moveDirection * movementSpeed;
        float speedDif = targetSpeed - rbVelocity;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.001f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * direction);
    }

    
    //Gets the mouse cursor from the screen position (bottom-left corner)
    public void OnLook(InputValue value)
    {
        Vector2 lookDir = value.Get<Vector2>();

        lookX = lookDir.x;
        lookY = lookDir.y;
    }
}
