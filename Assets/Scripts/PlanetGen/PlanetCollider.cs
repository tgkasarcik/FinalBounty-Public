using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetCollider : MonoBehaviour
{
    private SolarSystemColliderHandler handlerScript;
    private bool isPlayerNear;
    private bool isPlanetCleared;
    private ProgressManager Progress = ProgressManager.GetInstance();

    void Awake()
    {
        isPlanetCleared = Progress.IsPlanetCleared(this.gameObject);
        if (!isPlanetCleared)
        {
            this.handlerScript = GameObject.Find("Scripts").GetComponent<SolarSystemColliderHandler>();
            isPlayerNear = false;
        }
    }

    void OnTriggerEnter(Collider collision){
        if(!isPlayerNear && !isPlanetCleared)
        {
            if(collision.CompareTag("Player")){

                handlerScript.AddPlanet(this.gameObject);
                isPlayerNear = true;

            }
        }
    }

    void OnTriggerExit(Collider collision){
        if(isPlayerNear && !isPlanetCleared)
        {
            if(collision.CompareTag("Player")){

                handlerScript.RemovePlanet(this.gameObject);
                isPlayerNear = false;
            }
        }
    }
}

