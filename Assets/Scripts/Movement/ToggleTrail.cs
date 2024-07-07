using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailToggle : MonoBehaviour
{

    private bool on = false;
    private TrailRenderer trailRenderer;

    void OnEnable()
    {
        on = false; 
        trailRenderer = gameObject.GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    public void ToggleTrail()
    {
        on = !on;
        trailRenderer.emitting = on;
    }
}
