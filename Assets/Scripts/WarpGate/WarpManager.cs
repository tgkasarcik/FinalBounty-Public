using PlanetGen;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class WarpManager : MonoBehaviour
{
    [SerializeField] GameObject GatePrefab;
    [SerializeField] Texture RedTexture;
    [SerializeField] Texture BlueTexture;
    [SerializeField] Texture RedLightTexture;
    [SerializeField] Texture BlueLightTexture;
    [SerializeField] Boolean isLevelCleared = false;
    WarpGate warpGate;
    private InputActions inputActions;
    private InputAction interactWarp;
    private Boolean isSafe;
    private Boolean flag = true;
    private SceneLoader loader;
    private readonly string WarpScene = "Warp";
    private ProgressManager progress;
    private void OnEnable()
    {
        //TODO Test
        inputActions = new InputActions();
        interactWarp = inputActions.Planet.Interact;
        interactWarp.performed += HandleInteractWarp;
        
    }
    void Start()
    {
        progress = new ProgressManager();

        if (isLevelCleared){
            progress.SetLevelCleared(); 
        }
        loader = new SceneLoader();
        warpGate = new(GatePrefab, RedTexture, BlueTexture, RedLightTexture, BlueLightTexture);
        SetNotSafe();
        warpGate.Spawn();
    }
    void Update()
    {
        if (!isSafe && PlanetRoomManager.IsLevelCleared())
        {
            Debug.Log("test");
            SetSafe();
        }
        warpGate.DisplayMessage();
    }
    //this should be called after there are no more enemies
    private void SetSafe()
    {
        interactWarp.Enable();
        warpGate.SetReady();
        isSafe = true;
    }
    //this should be called at the start of the level, with enemies
    public void SetNotSafe()
    {
        interactWarp.Disable();
        warpGate.SetNotReady();
        isSafe = false;
    }
    private void HandleInteractWarp(InputAction.CallbackContext obj)
    {
        int level = PlayerLevelManager.GetCurrentLevel();
        if (isSafe && warpGate.IsInPosition() && flag)
        {
            loader.StartLoad(WarpScene, false);
            
            interactWarp.Disable();
        }
        flag = !flag;
    }
    

    
    
}
