using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Projectiles : MonoBehaviour
{
    private GameObject Player;
    private float bufferValue = 2;
    [SerializeField] GameObject playerProjectile;
    [SerializeField] public GameObject Planet;
    [SerializeField] bool UseGravity;
    private Rigidbody projectileRb;
    public float projectileSpeed = 1f;
    private float gunHeat;
    public float fireRate = .25f;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Debug.Log(Player);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //https://forum.unity.com/threads/how-can-i-add-a-fire-rate-time-for-the-shooting-projectile.904001/
        if (gunHeat > 0)
        {
            gunHeat -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            if (gunHeat <= 0)
            {
                Shoot();
                gunHeat += fireRate;
            }
        }
    }

    private void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 cameraToPlayer = Camera.main.transform.position - Player.transform.position;

        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(
             new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane + cameraToPlayer.magnitude));
        Vector3 direction = (clickPosition - Player.transform.position).normalized;
        
        GameObject projectile = Instantiate(playerProjectile, Player.transform.position + (direction * bufferValue), Quaternion.identity);
        projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 shootForce = direction * projectileSpeed + Player.GetComponent<Rigidbody>().velocity;
        projectileRb.AddForce(shootForce, ForceMode.VelocityChange);

        //wrap around planet if specified
        if(UseGravity)
        {
            GravityBody gravBod = projectile.AddComponent<GravityBody>();
            gravBod.SetPlanetObj(Planet);
        }

    }

}

