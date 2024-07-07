using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerObject;

public class CycleWeaponHandler : MonoBehaviour
{
    private InputActions inputActions;
	private InputAction up;
    private InputAction down;

    private IPlayer player;

	private void OnEnable()
	{
        this.player = PlayerManager.FindPlayerByObject(this.gameObject);
		inputActions = new InputActions();
		up = inputActions.Player.CycleWeaponUp;
		up.Enable();
        up.performed += HandleWeaponUp;

        down = inputActions.Player.CycleWeaponDown;
		down.Enable();
		down.performed += HandleWeaponDown;
	}


	private void HandleWeaponUp(InputAction.CallbackContext obj){
        this.player.projectiles.CycleGunUp();
	}

    private void HandleWeaponDown(InputAction.CallbackContext obj){
		this.player.projectiles.CycleGunDown();
	}

	private void OnDisable()
	{
        up.Disable();
		down.Disable();
	}
}