using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WarpGate
{
    private GameObject GatePrefab;
    private Texture RedTexture; 
    private Texture BlueTexture; 
    private Texture RedLightTexture;
    private Texture BlueLightTexture;
    private GameObject Gate;
    private Material sharedMaterial;
    private Boolean isReady;
    private Boolean inPosition;
    private WarpCollision collider;
    private string message;
    private GameObject canvasGO;
    private GameObject textGO;
    private Text text;
    private Canvas canvas;
    //private Text object

    public WarpGate(GameObject GatePrefab, Texture RedTexture, Texture BlueTexture, Texture RedLightTexture, Texture BlueLightTexture)
    {
        this.GatePrefab= GatePrefab;
        this.RedTexture= RedTexture;
        this.BlueTexture= BlueTexture;
        this.RedLightTexture= RedLightTexture;
        this.BlueLightTexture= BlueLightTexture;
        Gate = Transform.Instantiate(this.GatePrefab, new Vector3(35, 2, 5), Quaternion.identity);
        sharedMaterial = Gate.GetComponent<MeshRenderer>().sharedMaterial;
        Gate.SetActive(true);
        collider = Gate.GetComponent<WarpCollision>();
        MakeMessage();

    }
    private void MakeMessage()
    {
        //https://docs.unity3d.com/ScriptReference/Canvas.html
        //Canvas
        canvasGO = new GameObject();
        canvasGO.name = "WarpCanvas";
        //canvasGO.AddComponent<Canvas>();
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        //Text
        textGO = new GameObject();
        textGO.transform.parent = canvasGO.transform;
        textGO.name = "WarpText";
        
        text = textGO.AddComponent<Text>();
        text.font = (Font)Resources.Load("Fonts/nasalization-rg");
        text.fontSize = 36;

        //Position 
        
        var rectTransform = textGO.GetComponent<RectTransform>();
       
        rectTransform.localPosition = new Vector3(6, 156, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);


    }
    public Boolean IsInPosition()
    {
        return inPosition;
    }
    //call in Update()
    public void DisplayMessage()
    {
        if (collider.GetWantWarp() && !canvasGO.activeSelf)
        {
            inPosition = true;
            canvasGO.SetActive(true);
        }//https://docs.unity3d.com/ScriptReference/GameObject-activeSelf.html
        else if (!collider.GetWantWarp() && canvasGO.activeSelf)
        {
            inPosition = false;
            canvasGO.SetActive(false);
        }
    }
    public void SetReady()
    {
        this.SetBlue(); 
        text.text = "Press E to warp";
        isReady = true;
    }

    public void SetNotReady()
    {
        this.SetRed();
        text.text = "Unable to warp with hostiles nearby";
        isReady = false;
    }
    private void SetRed()
    {
        sharedMaterial.SetVector("_RimColor", Color.red);
        sharedMaterial.SetVector("_EmissiveColor", Color.red);
        sharedMaterial.SetTexture("_Texture", this.RedTexture);
        sharedMaterial.SetTexture("_Emissive", this.RedLightTexture);
    }

    private void SetBlue()
    {
        sharedMaterial.SetVector("_RimColor", Color.cyan);
        sharedMaterial.SetVector("_EmissiveColor", Color.cyan);
        sharedMaterial.SetTexture("_Texture", this.BlueTexture);
        sharedMaterial.SetTexture("_Emissive", this.BlueLightTexture);
    }
    public void Spawn()
    {
        Gate.SetActive(true);
    }
    
    public void Despawn()
    {
        Gate.SetActive(false);
    }
}
