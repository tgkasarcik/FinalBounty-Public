using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopShipCollider : MonoBehaviour
{
    private SolarSystemColliderHandler handlerScript;
    private bool isPlayerNear;

    void Awake()
    {
        this.handlerScript = GameObject.Find("Scripts").GetComponent<SolarSystemColliderHandler>();
        isPlayerNear = false;
    }

    void OnTriggerEnter(Collider collision){
    if(!isPlayerNear)
    {
        if(collision.CompareTag("Player")){
            this.handlerScript.isNearShop(true);
            isPlayerNear = true;
            }
        }
    }

    void OnTriggerExit(Collider collision){
    if(isPlayerNear)
    {
        if(collision.CompareTag("Player")){
            this.handlerScript.isNearShop(false);
            isPlayerNear = false;
            }
        }
    }
}


