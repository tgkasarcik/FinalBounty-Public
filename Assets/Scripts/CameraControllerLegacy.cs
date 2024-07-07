using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerLegacy : MonoBehaviour
{
    [SerializeField] public GameObject Player;

    private Vector3 offset;

    // random comment to test committing
    void Start()
    {
        offset = transform.position - Player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Player.transform.position + offset;
    }
}
