using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCollision : MonoBehaviour
{
    private Boolean wantWarp = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wantWarp = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wantWarp = false;
        }
    }
    public Boolean GetWantWarp()
    {
        return wantWarp;
    }
}
