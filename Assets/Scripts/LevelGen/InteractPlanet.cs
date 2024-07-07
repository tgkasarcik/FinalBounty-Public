using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractSolarSystem : MonoBehaviour
{
    private InputActions inputActions;
	private InputAction interactPlanet;
    private SolarSystemColliderHandler planetCol;

	private void OnEnable()
	{
        planetCol = this.gameObject.GetComponent<SolarSystemColliderHandler>();
		inputActions = new InputActions();
		interactPlanet = inputActions.Planet.Interact;
		interactPlanet.Enable();
		interactPlanet.performed += HandleInteractPlanet;
	}

	private void HandleInteractPlanet(InputAction.CallbackContext obj){
		planetCol.EnterObject();
	}

	private void OnDisable()
	{
		interactPlanet.Disable();
	}
}
