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

public static class HUDNotifications
{
    private static GameObject notificationsObj;
    private static GameObject panel;
    private static HUDNotificationsMono mono;
    private static bool didInit = false;
    private static List<Notification> notificationsMade;

    public static void Initialize(GameObject notificationsObjTemp, GameObject panelTemp, HUDNotificationsMono monoTemp)
    {
        if(!didInit)
        {
            notificationsMade = new List<Notification>();
        }
        notificationsObj = notificationsObjTemp;
        mono = monoTemp;
        panel = panelTemp;
        didInit = true;
    }

    public static void MakeNotification(string text)
    {
        if(didInit)
        {
            Notification newNotif = new Notification(text);
            newNotif.CreateObject(notificationsObj, panel.transform);
            notificationsMade.Add(newNotif);
        }
    }

    public static void changeInit(bool val)
    {
        didInit = val;
    }

    public static void MakeSavedNotifications()
    {
        foreach(Notification notification in notificationsMade)
        {
            notification.CreateObject(notificationsObj, panel.transform);
        }
    }

    public static Notification GetNotification(GameObject objToCheck)
    {
        foreach(Notification notification in notificationsMade)
        {
            if(objToCheck == notification.notifObj)
            {
                return notification;
            }
        }
        return null;
    }

    public static void RemoveSavedNotification(Notification notif)
    {
        notificationsMade.Remove(notif);
    }

}
