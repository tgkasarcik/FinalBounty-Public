using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookHandler
{
    Movement movement;
    public LookHandler(InputAction switchAction, Movement movement)
    {
        this.movement = movement;
        switchAction.performed += Look_performed;
        switchAction.Enable();
    }

    private void Look_performed(InputAction.CallbackContext obj)
    { 
        //movement.Look2D(obj.ReadValue<Vector2>());
    }

}
