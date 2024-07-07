using JetBrains.Annotations;
using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BossHealth : MonoBehaviour
{
    public static event Action<float> bossHealth;
    public static event Action<bool> bossVictory;

    public float totalHealth = 0f;
    public float maxHealth;

    private GameObject explosionFX;
    private bool defeated = false;
    private float bossExplosionChance = 0.4f;
    private float timeBetweenExplosionChecks = 0.05f;

    private AudioManager _audioManager;

    void Awake()
    {
        explosionFX = (GameObject)Resources.Load("Prefabs/Particles/FX_Explosion");
        _audioManager = FindObjectOfType<AudioManager>();

        BossWeakSpot[] weakSpots = gameObject.GetComponentsInChildren<BossWeakSpot>();
        foreach(BossWeakSpot weakSpot in weakSpots)
        {
            totalHealth += weakSpot.Health;
        }

        maxHealth = totalHealth;

        BossWeakSpot.bossTakeDamage += TakeDamage;
    }

    public void TakeDamage(float damage)
    {
        totalHealth -= damage;

        //Invokes an event to tell the health bar to decrease to this amount
        float healthBarPercent = (totalHealth / maxHealth) * 100;
        bossHealth?.Invoke(healthBarPercent);

        if(totalHealth <= 0f)
        {
            if(!defeated)
            {
                defeated = true;
                BossCutScene();
            }
        }
    }

    public void BossCutScene()
    {
        Camera camera = Camera.main;

        //Makes players invunerable during the cutscene
        foreach (IPlayer player in PlayerManager.players)
        {
            player.playerObj.GetComponent<MeshCollider>().enabled = false;
        }

        //Gives the camera the necessary functions to orbit for the cutscene
        Rigidbody rb = camera.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.mass = 1f;

        CameraController controller = camera.GetComponent<CameraController>();
        controller.bossCinematic = true;

        GravityBody gravityBody = camera.AddComponent<GravityBody>();
        gravityBody.ShouldRotate = false;

        gravityBody.planetObj = gameObject;
        rb.AddForce(camera.transform.right * 1000f, ForceMode.Acceleration);


        Gravity gravity = GetComponent<Gravity>();
        gravity.distance = gravity.distance * 3;
        //gravity.repulsiveStrength = 50f;

        StartCoroutine(BossEffects());
    }

    IEnumerator ExplosionEffects()
    {

        //Using the surrounding cells, a racast is shot from the shell towards the middle
        //The location of the hit racast will then spawn an explosion
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");

        foreach (GameObject temp in cells)
        {
            //Gets a random cell from the cell list
            int randNumCell = UnityEngine.Random.Range(0, cells.Length - 1);
            GameObject cell = cells[randNumCell];

            //Checks where the cell hits the boss and where to place the explosion effect
            Debug.DrawRay(cell.transform.position, -cell.transform.up, Color.red, 10f);
            RaycastHit hit;
            if (Physics.Raycast(cell.transform.position, -cell.transform.up, out hit, 1000f))
            {
                //Random chance if the racast hit will explode
                float randNumChance = UnityEngine.Random.value;
                if (randNumChance < bossExplosionChance)
                {
                    Instantiate(explosionFX, hit.point, Quaternion.identity);
                    _audioManager.PlayRandomExplosion();
                }
            }
            yield return new WaitForSeconds(timeBetweenExplosionChecks);
        }


    }

    //Makes the boss explode, then gives the player feedback, then goes back to the solar
    IEnumerator BossEffects()
    {
        StartCoroutine(ExplosionEffects());
        yield return new WaitForSeconds(5f);
        bossVictory?.Invoke(true);
        yield return new WaitForSeconds(3f);

        StopCoroutine(ExplosionEffects());
        bossVictory?.Invoke(false);

        ProgressManager progManager = ProgressManager.GetInstance();
        progManager.SetLevelCleared();
        progManager.SetPlanetCleared();
        PlanetRoomManager.ExitPlanet();
    }



    private void OnDisable()
    {
        BossWeakSpot.bossTakeDamage -= TakeDamage;
    }

}
