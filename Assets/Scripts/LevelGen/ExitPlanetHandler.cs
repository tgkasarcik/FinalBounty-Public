using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitPlanetHandler : MonoBehaviour
{
    private InputActions inputActions;
	private InputAction exit;

	private void OnEnable()
	{
		inputActions = new InputActions();
		exit = inputActions.Planet.Exit;
		exit.Enable();
		exit.performed += HandleExit;
	}
    private void HandleExit(InputAction.CallbackContext obj){
		PlanetRoomManager.ExitPlanet();
	}

	private void OnDisable()
	{
		exit.Disable();
	}
}

