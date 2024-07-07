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

//object has info on notification
public class Notification
{
    public GameObject notifObj;
    private string notifText;

    public Notification(string notifText)
    {
        this.notifText = notifText;
    }

    public void CreateObject(GameObject notifObjPrefab, Transform parentTransform)
    {
        this.notifObj = GameObject.Instantiate(notifObjPrefab);
        this.notifObj.transform.parent = parentTransform;
        this.notifObj.transform.localScale = Vector3.one;
        this.notifObj.SetActive(true);
        var destroy = this.notifObj.AddComponent<FadeOutElementOverTime>();
        destroy.TotalTime = 4f;
        destroy.FadeOutTime = .5f;
        TextMeshProUGUI textObj = this.notifObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        textObj.text = notifText;
    }
    

}
