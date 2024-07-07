using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBox : MonoBehaviour
{
    [SerializeField] Material[] LevelSkyboxes;
    // Start is called before the first frame update
    void Start()
    {
        int level = PlayerLevelManager.GetCurrentLevel();
        RenderSettings.skybox= LevelSkyboxes[level];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
