using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarSystemColliderHandler : MonoBehaviour
{
    public bool nearBoss;
    public bool nearPlanet;
    public bool nearShop;

    private SceneLoader loader;
    private HUDInteractions HUD;
    public List<GameObject> closestPlanet;
    public GameObject enterPlanetTextObj;
    public TextMeshProUGUI enterPlanetText;

    void Awake()
    {
        this.nearBoss = false;
        this.nearShop = false;
        this.nearPlanet = false;
        this.closestPlanet = new List<GameObject>();
        var canvasObject = GameObject.Find("Canvas");
        this.enterPlanetTextObj.SetActive(false);
    }
    void Start()
    {
        GameObject hudScripts = GameObject.Find("HUDScripts");
        this.HUD = hudScripts.GetComponent<HUDInteractions>();
    }

    public void EnterObject()
    {
        if(nearShop)
        {
            this.HUD.UpgradesButton_OnClick();
        }
        else if(nearPlanet || nearBoss)
        {
            //enters last planet player got near
            PlanetRoomManager.EnterPlanet(closestPlanet[closestPlanet.Count - 1]);
        }
    }



    public void AddPlanet(GameObject Planet)
    {
        closestPlanet.Add(Planet);

        //enable UI and look for input if first to push
        if(closestPlanet.Count == 1)
        {
            this.nearPlanet = true;
            this.enterPlanetTextObj.SetActive(true);
        }
    }

    public void RemovePlanet(GameObject Planet)
    {
        closestPlanet.Remove(Planet);

        //disable UI and look for input if the last one to pop
        if(closestPlanet.Count == 0)
        {
            this.nearPlanet = false;
            if(!this.nearShop)
            {
                this.enterPlanetTextObj.SetActive(false);
            }
        }
    }

    public void isNearShop(bool isNear)
    {
        this.nearShop = isNear;
        if(isNear)
        {
            //enable text and set text to enter shop instead (as it takes priority)
            this.enterPlanetText.text = "Press E to enter shop";
            this.enterPlanetTextObj.SetActive(true);
        }
        else if(this.nearPlanet)
        {
            //dont disable and set text to planet if still near a planet
            this.enterPlanetText.text = "Press E to enter planet";
        }
        else
        {
            //if not near a planet or shop, disable the text
            this.enterPlanetText.text = "Press E to enter planet";
            this.enterPlanetTextObj.SetActive(false);
        }
    }

    public void isNearBoss(bool isNear)
    {
        this.nearBoss = isNear;
        if (isNear)
        {
            //enable text and set text to enter shop instead (as it takes priority)
            this.enterPlanetText.text = "Press E to fight boss";
            this.enterPlanetTextObj.SetActive(true);
        }
        else if (this.nearPlanet)
        {
            //dont disable and set text to planet if still near a planet
            this.enterPlanetText.text = "Press E to enter planet";
        }
        else
        {
            //if not near a planet or shop, disable the text
            this.enterPlanetText.text = "Press E to enter planet";
            this.enterPlanetTextObj.SetActive(false);
        }
    }
}
