using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Upgrades;
using UnityEngine.InputSystem;

public class WaypointsMono : MonoBehaviour
{
    [SerializeField] public Transform CanvasObj;
	[SerializeField] public bool EnabledByDefault;
    private InputAction waypointToggleAction;

    void Awake()
    {
        WaypointManager.Initialize(CanvasObj);
        waypointToggleAction = InputManager.GetInputActions().UI.Waypoints;
		waypointToggleAction.performed += WaypointToggleAction_performed;
        waypointToggleAction.Enable();
		CanvasObj.gameObject.SetActive(EnabledByDefault);
    }

	private void OnDisable()
	{
		waypointToggleAction.Disable();
	}

	private void WaypointToggleAction_performed(InputAction.CallbackContext obj)
	{
		CanvasObj.gameObject.SetActive(!CanvasObj.gameObject.activeSelf);
	}
}
