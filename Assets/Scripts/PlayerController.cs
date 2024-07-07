using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	public float Velocity = 0;

	public event EventHandler<Collider> PowerUpCollision;
	private InputActions inputActions;
	private InputAction movement;
	private InputAction fire;
	private Rigidbody rb;

	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		inputActions = new InputActions();
		movement = inputActions.Player.Move;
		movement.Enable();
		fire = inputActions.Player.Fire;
		fire.Enable();
	}

	private void FixedUpdate()
	{
		var inputMovement = movement.ReadValue<Vector2>();
		var worldMovement = new Vector3(inputMovement.x, 0.0f, inputMovement.y);
		rb.AddForce(worldMovement * Velocity);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pickup"))
		{
			OnPowerUpCollision(other);
		}
	}

	protected virtual void OnPowerUpCollision(Collider powerUpCollider)
	{
		PowerUpCollision?.Invoke(this, powerUpCollider);
	}

	private void OnDisable()
	{
		movement.Disable();
	}
}
