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

public static class WaypointManager
{
    private static GameObject notificationsObj;
    private static Transform canvas;
    private static HUDNotificationsMono mono;
    private static bool didInit = false;
    private static List<Notification> notificationsMade;

    private static GameObject ItemWaypoint;
    private static GameObject ShopWaypoint;
    private static GameObject PortalWaypoint;
    private static GameObject BossWaypoint;
    private static GameObject ClearedWaypoint;

    public static void Initialize(Transform canvasObj)
    {
        canvas = canvasObj;
        didInit = true;
        ItemWaypoint = (GameObject)Resources.Load("Prefabs/Waypoints/Item_Waypoint");
        ShopWaypoint = (GameObject)Resources.Load("Prefabs/Waypoints/Chest_Waypoint");
        PortalWaypoint = (GameObject)Resources.Load("Prefabs/Waypoints/Portal_Waypoint");
        BossWaypoint = (GameObject)Resources.Load("Prefabs/Waypoints/Boss_Waypoint");
        ClearedWaypoint = (GameObject)Resources.Load("Prefabs/Waypoints/Cleared_Waypoint");
    }

    public static void MakeWaypoint(GameObject trackedObj, string type, string text)
    {
        if(didInit)
        {
            GameObject prefab;
            switch(type.ToLower())
            {
                case "item":
                {
                    Waypoint waypoint = trackedObj.AddComponent<Waypoint>();
                    waypoint.Initialize(ItemWaypoint, canvas, text);
                    break;
                }
                case "shop":
                {
                    Waypoint waypoint = trackedObj.AddComponent<Waypoint>();
                    waypoint.Initialize(ShopWaypoint, canvas, text);
                    break;
                }
                case "portal":
                {
                    Waypoint waypoint = trackedObj.AddComponent<Waypoint>();
                    waypoint.Initialize(PortalWaypoint, canvas, text);
                    break;
                }
                case "boss":
                {
                    Waypoint waypoint = trackedObj.AddComponent<Waypoint>();
                    waypoint.Initialize(BossWaypoint, canvas, text);
                    break;
                }
                case "cleared":
                {
                    Waypoint waypoint = trackedObj.AddComponent<Waypoint>();
                    waypoint.Initialize(ClearedWaypoint, canvas, text);
                    break;
                }
                default:
                {
                    //return if not a valid type
                    Debug.Log("ERROR: not a valid waypoint type...Halting creation");
                    return;
                }
            }
        }
    }

    public static void changeInit(bool val)
    {
        didInit = val;
    }

}
