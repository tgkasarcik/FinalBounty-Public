using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] public GameObject Sphere;
    [SerializeField] public bool orbiting;
    [SerializeField] public float cameraDistance;

    public bool bossCinematic = false;
    private Vector3 offset;
    

    // random comment to test committing
    void OnEnable()
    {
        offset = new Vector3(0f, transform.position.y - Player.transform.position.y, 0f);
        Player = GameObject.FindWithTag("Player");
    }

    void LateUpdate()
    {
        if(orbiting)
        {
            if (!bossCinematic)
            {
                //Positions the camera above the player
                Vector3 normal = Player.transform.position - Sphere.transform.position;
                Vector3 planetDistance = normal.normalized * cameraDistance;
                Vector3 target = Player.transform.position + planetDistance;
                transform.position = target;
            }

            //Rotates the camera to look at the player
            Vector3 planetToCamera = Sphere.transform.position - transform.position;
            Quaternion q = Quaternion.FromToRotation(transform.forward, planetToCamera) * transform.rotation;
            transform.rotation = q;

        }
        else
        {
            //Camera does not rotate if it's not orbiting
            transform.position = Player.transform.position + offset;
        }

        
    }
}
