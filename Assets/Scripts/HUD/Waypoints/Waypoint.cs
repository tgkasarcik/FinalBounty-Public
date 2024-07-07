using UnityEngine;
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

//https://www.youtube.com/watch?v=s7ZBXa5GSA8
public class Waypoint : MonoBehaviour
{
    public GameObject prefab;

    private RectTransform waypoint;

    private GameObject waypointObj;

    private Transform WaypointCanvas;

    private string inputText;

    public void Initialize(GameObject prefab, Transform WaypointCanvas, string inputText)
    {
        this.prefab = prefab;
        this.WaypointCanvas = WaypointCanvas;
        this.inputText = inputText;
    }

    void Start()
    {
        waypointObj = Instantiate(this.prefab, WaypointCanvas);
        waypointObj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inputText;
        waypoint = waypointObj.GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 playerPos = PlayerManager.players[0].playerObj.transform.position;
        //gets player distance from object attached
        Vector3 distance = this.transform.position - playerPos;
        //Debug.Log(screenPos);
        waypoint.position = screenPos;//new Vector3(Mathf.Clamp(screenPos.x, 0, Screen.width), Mathf.Clamp(screenPos.x, 0, Screen.height), screenPos.z);
        TextMeshProUGUI textObj = waypointObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        //update text field
        textObj.text = (int)distance.magnitude + " m";
    }

    void OnDisable()
    {
        //Destroy(waypoint);
    }

}