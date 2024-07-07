using PlanetGen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTexture : MonoBehaviour
{
    private Material material;
    private Color OriginalColor;
    // Start is called before the first frame update
    void Start()
    {
        ProgressManager progress = ProgressManager.GetInstance();
        material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        OriginalColor = material.GetColor("_RimColor");
        if (progress.IsPlanetCleared(this.gameObject))
        {
            material.SetColor("_RimColor", Color.white);
        }

    }
    private void OnDisable()
    {
        material.SetColor("_RimColor", OriginalColor);
    }


}
