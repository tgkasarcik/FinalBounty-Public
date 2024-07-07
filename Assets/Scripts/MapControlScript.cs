using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapControlScript : MonoBehaviour
{
	private InputActions inputActions;
	private InputAction switchRooms;
	private SceneLoader loader;
	private void OnEnable()
	{
		inputActions = new InputActions();
		switchRooms = inputActions.Planet.ChangeRoom;
		switchRooms.Enable();
		switchRooms.performed += HandleSwitchRooms;
        loader = new SceneLoader();
    }

	private void HandleSwitchRooms(InputAction.CallbackContext obj){
        //PlayerLevelManager.moveRandom();
        //Debug.Log("Switched rooms");
        loader.StartLoad("Warp");
    }

	private void OnDisable()
	{
		switchRooms.Disable();
	}
}
