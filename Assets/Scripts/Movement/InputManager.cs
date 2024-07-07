using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    [SerializeField] private ZoomHandler zoomHandler;
    //[SerializeField] private Movement playerMovement;
    private static InputActions InputActions;

    private void Awake()
    {
        InputActions = new InputActions();
    }

    private void OnEnable()
    {
        //_ = new MovementHandler(inputActions.Player.Move, playerMovement);
        //_ = new LookHandler(inputActions.Player.Look, playerMovement);
        //_ = new QuitHandler(InputActions.UI.Quit);
        zoomHandler.Initialize(InputActions.Player.Zoom);
        //_ = new HighSpeedHandler(inputActions.Player.HighSpeed, playerMovement);
    }

    public static InputActions GetInputActions()
    {
        return InputActions;
    }
}
