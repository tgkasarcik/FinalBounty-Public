using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class ExitControls : MonoBehaviour
{
    [SerializeField] public string SceneToLoadOnEsc;
    
    private InputActions inputActions;
    private InputAction exit;
    private bool exiting;
    void Start()
    {
        inputActions = new InputActions();
        exit = inputActions.UI.Quit;
        exit.Enable();
        exiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Keeps multiple exit scenes from being loaded
        Scene exitScene = SceneManager.GetSceneByName(SceneToLoadOnEsc);
        if (exit.IsPressed() && !exiting && SceneManager.GetActiveScene() != exitScene && !exitScene.isLoaded)
        {
            SceneManager.LoadScene(SceneToLoadOnEsc, LoadSceneMode.Additive);
            exiting = true;
        }
        else if (!exit.IsPressed() && exiting) 
        {
            exiting = false;
        }

        if (exitScene.isLoaded)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
