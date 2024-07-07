using PlayerObject;
using System;
using System.Collections;
using System.Xml.XPath;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class MovementWithAbilities : Movement
{
    [SerializeField] public float dashMultiplier = 2f;
    [SerializeField] public float rotation = 90f;
    [SerializeField] public float rotationSpeed = 1.0f;
    [SerializeField] public float doubleTapTime = 0.5f;
    [SerializeField] public float dashCoolDown = 0.5f;
    [SerializeField] public float rollCoolDown = 1.0f;

    [SerializeField] public UnityEvent wingTrailTrigger;
    [SerializeField] public UnityEvent mainTrailTrigger;

    private float tailTogglerTimer = 0.2f;

    IPlayer playerShip;

    float timeSinceLastClicked = 0;
    bool dashOnCoolDown = false;
    bool rollOnCoolDown = false;
    private Vector2 lastMoveInput = new Vector2();

    private void OnMove(InputValue value)
    {
        Vector2 moveDir = value.Get<Vector2>();

        movementX = moveDir.x;
        movementZ = moveDir.y;

        //Calculates if the player has input or can dash
        if (moveDir != Vector2.zero)
        {
            if (Time.time - timeSinceLastClicked < doubleTapTime && !dashOnCoolDown && moveDir == lastMoveInput)
            {
                Dash();
                dashOnCoolDown = true;
                StartCoroutine(CoolDownTimer (dashCoolDown, timer => { dashOnCoolDown = timer; }));
            }
            timeSinceLastClicked = Time.time;
            lastMoveInput = moveDir;
        }
    }

    //Makes the player move faster
    public void OnHighSpeed(InputValue value)
    {
        
        movementSpeed = value.isPressed ? movementSpeed * movementModifier : movementSpeed / movementModifier;
    }

    //Generic method for cool downs
    private IEnumerator CoolDownTimer(float time, Action<bool> coolDownTimer)
    {
        yield return new WaitForSeconds(time);
        if (coolDownTimer != null) coolDownTimer(false);
    }

    private void OnMovementAbility()
    {
        if (!rollOnCoolDown)
        {
            BarrelRoll();
            rollOnCoolDown = true;
            StartCoroutine(CoolDownTimer (rollCoolDown, timer => { rollOnCoolDown = timer; }));
        }
        
        Debug.Log("Movement Ability pressed");
    }

    private void OnChangeAbility()
    {
        Debug.Log("Ability Changed");
    }

    private void Dash()
    {
        mainTrailTrigger.Invoke();

        multiplyMovementVals(dashMultiplier);
        MovePhysicsBased();
        multiplyMovementVals(1/dashMultiplier);

        StartCoroutine(TurnOffTrail());
    }

    IEnumerator TurnOffTrail()
    {
        yield return new WaitForSeconds(tailTogglerTimer);
        mainTrailTrigger.Invoke();
    }

    private void BarrelRoll()
    {
        if(playerShip == null)
        {
            playerShip = PlayerManager.FindPlayerByObject(gameObject);
        }

        StartCoroutine(Rotate(rotation, rotationSpeed));
    }

    //Code from https://stackoverflow.com/questions/53106326/quaternion-slerp-not-smoothly-rotating-for-some-reason
    public IEnumerator Rotate(float rotateAmount, float rotationSpeed)
    {
        RotateEffects(true);

        Quaternion rotation = new Quaternion();
        float beginningAngle = transform.eulerAngles.z;

        //Lerps between a begninning angle and ending angle
        for (float t = 0; t <= 1.0; t += Time.deltaTime * rotationSpeed)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(beginningAngle, beginningAngle + rotateAmount, t));
            rotation = transform.rotation;
            yield return null;
        }

        RotateEffects(false);

        yield return rotation;

    }

    private void RotateEffects(bool on)
    {
        wingTrailTrigger.Invoke();
        playerShip.invunerable = on;
        playerShip.rolling = on;

        //Should rotate must be turned off in order for the player to break out of the gravity 
        gBody.ShouldRotate = !on;
    }

    private void multiplyMovementVals(float multiplier)
    {
        movementSpeed *= multiplier;
        acceleration *= multiplier;
        decceleration *= multiplier;
    }
}
