using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMenuInteractions : MonoBehaviour
{
    [SerializeField] public string GameScene;
    [SerializeField] public string ExitScene;
    [SerializeField] public string StartScene;

    public void No_Button_OnClick()
    {
        SceneManager.UnloadSceneAsync(ExitScene);
    }

    public void Yes_Button_OnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        SceneManager.UnloadSceneAsync(GameScene);
        SceneManager.UnloadSceneAsync(ExitScene);
        Application.Quit();
    }

    public void Reset_Button_OnClick()
    {
        SceneManager.UnloadSceneAsync(GameScene);
        SceneManager.UnloadSceneAsync(ExitScene);
        SceneManager.LoadSceneAsync(StartScene);
        //regenerate graph?
    }
}
