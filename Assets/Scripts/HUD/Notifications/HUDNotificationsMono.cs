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

public class HUDNotificationsMono : MonoBehaviour
{
    [SerializeField] public GameObject NotificationsPanel;
    [SerializeField] public GameObject NotificationsObj;

    void Awake()
    {
        HUDNotifications.Initialize(NotificationsObj, NotificationsPanel, this);
        HUDNotifications.MakeSavedNotifications();
    }

}
