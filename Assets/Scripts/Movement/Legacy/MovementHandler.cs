using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHandler
{
    Movement movement;
    public MovementHandler(InputAction switchAction, Movement movement)
    {
        this.movement = movement;
        switchAction.performed += Movement_performed;
        switchAction.canceled += Movement_canceled;
        switchAction.Enable();
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        //movement.Move(obj.ReadValue<Vector2>());
    }

    private void Movement_canceled(InputAction.CallbackContext obj)
    {
        //movement.Move(obj.ReadValue<Vector2>());
    }
}
