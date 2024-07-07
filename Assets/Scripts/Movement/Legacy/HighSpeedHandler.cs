using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighSpeedHandler
{
    Movement movement;

    public HighSpeedHandler(InputAction switchAction, Movement movement)
    {
        this.movement = movement;
        switchAction.performed += HighSpeed_performed;
        switchAction.canceled += HighSpeed_performed;
        switchAction.Enable(); 
    }

    private void HighSpeed_performed(InputAction.CallbackContext obj)
    {
        Debug.Log($"High Speed performed: {obj.ReadValueAsButton()}");
        //movement.HighSpeed(obj.ReadValueAsButton());
    }
}
