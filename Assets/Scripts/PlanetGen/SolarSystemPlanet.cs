using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

//https://github.com/SebLague/Spherical-Gravity/blob/master/GravityAttractor.cs
public class SolarSystemPlanet : MonoBehaviour
{
    public double gravity = 9.8f;

    private GameObject planet;

    public double mass;
    public double radius;
    public double time;
    public double distanceFromSun;
    //orbit speed is in radians per second
    public double orbitSpeed;
    //rotation speed is in radians per second
    public double rotationSpeed;

    private System.Random rnd;
    private Vector3 sunPos;
    private bool useRandomTime;

    public void Initialize(double mass, double radius, Vector3 sunPos, double orbitSpeed, double rotationSpeed, double gravity = 9.8f, bool useRandomTime = true)
    {
        this.mass = mass;
        this.sunPos = sunPos;
        this.radius = radius;
        this.distanceFromSun = Vector3.Distance(sunPos, this.gameObject.transform.position);
        this.orbitSpeed = orbitSpeed;
        this.rotationSpeed = rotationSpeed;
        this.gravity = gravity;
        this.useRandomTime = useRandomTime;
    }
    void Awake()
    {
        this.rnd = new System.Random();
    }

    void Start()
    {
        //get random starting point on circle/orbit equation
        this.planet = this.gameObject;
        if(useRandomTime)
        {
            this.time = (double)rnd.Next();
        }
        this.planet.transform.position = getNewPos();
    }

    void Update()
    {
        //calculate new time and position
        this.time += Time.deltaTime;
        this.planet.transform.position = getNewPos();
        this.planet.transform.rotation = getNewRotation();
    }

    //gets new orbit position
    private Vector3 getNewPos()
    {
        float xPos = sunPos.x + (float)(distanceFromSun * Math.Cos(this.time * orbitSpeed));
        float zPos = sunPos.z + (float)(distanceFromSun * Math.Sin(this.time * orbitSpeed));
        return new Vector3(xPos, this.planet.transform.position.y, zPos);
    }

    //gets new rotation of planet (only on Y axis, try to implement tilted axis later)
    private Quaternion getNewRotation()
    {
        //reverse direction by reversing time variable
        float yRot = (float)((-this.time * rotationSpeed ) % 360);
        return Quaternion.Euler(0, yRot, 0);
    }
}