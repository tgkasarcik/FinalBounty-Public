using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTexture : MonoBehaviour
{
    private ParticleSystem.MainModule rays;
    private ParticleSystem.MinMaxGradient OriginalRay;
    private Color OriginalEmmision;
    private Material material;
    void Start()
    {
        material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        ProgressManager progress = ProgressManager.GetInstance();
        rays = gameObject.GetComponentInChildren<ParticleSystem>().main;
        OriginalRay = rays.startColor;
        OriginalEmmision = material.GetColor("_EmissionColor");
        if (progress.IsPlanetCleared(this.gameObject))
        {
            rays.startColor = new Color(0,143,255,255);
            material.SetColor("_EmissionColor", new Color(0, 105, 255) * .01f);
        }

    }

    private void OnDisable()
    {
        rays.startColor = OriginalRay;
        material.SetColor("_EmissionColor", OriginalEmmision);
    }


}
