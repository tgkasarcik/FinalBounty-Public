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

public class FadeOutElementOverTime : MonoBehaviour
{
    public float FadeOutTime = 1f;
    private float fadeTimer = 0f;
    public float TotalTime = 5f;
    private float timer = 0f;
    private float timeTillFade;

    private CanvasGroup cvsGrp;

    void Start()
    {
        cvsGrp = GetComponent<CanvasGroup>();
        timeTillFade = TotalTime - FadeOutTime;
        if(timeTillFade < 0)
        {
            timeTillFade = 0;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeTillFade)
        {
            cvsGrp.alpha = 1 - ((timer - timeTillFade)/ FadeOutTime);
        }

        if(timer >= TotalTime)
        {
            HUDNotifications.RemoveSavedNotification(HUDNotifications.GetNotification(this.gameObject));
            Destroy(this.gameObject);
        }
    }

}