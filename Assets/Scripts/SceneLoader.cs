using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string LoadingScreenName = "LoadingScreen";
    private static Image progressBar;
    public static string sceneToLoad;
    private static bool didInitLoadingScreen;
    private static bool didLoadScene = true;
    private static bool didStartLoading;
    private static bool didStartLoad;
    private static bool includeHUD;

    private AudioManager _audioManager;

    void Awake()
    {
        //checks if object is the only one allowed for the game and destroys it if not
        if(this.gameObject.tag == "SceneLoader")
        {
            if (this.gameObject.scene.buildIndex != -1)
            {
                if(GameObject.FindGameObjectsWithTag("SceneLoader").Length > 1)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }

    public void Initialize(string LoadingScreenName){
        this.LoadingScreenName = LoadingScreenName;
        didInitLoadingScreen = false;
    }

    public void Initialize(){
        didInitLoadingScreen = false;
        didStartLoad = false;
        _audioManager = FindAnyObjectByType<AudioManager>();
    }
    public bool IsSceneLoaded()
    {
        return didLoadScene;
    }
    void Update()
    {
        if(!didLoadScene)
        {
            //if the scene is still not on the loading screen, transfers scene to the loading screen
            //next, if the loading screen was just initialized, gets the progress bar to update it
            //next, loads the level specified when done with the last two tasks
            if(SceneManager.GetActiveScene().name != this.LoadingScreenName)
            {
                //Scene loadingScene = SceneManager.GetSceneByName(this.LoadingScreenName);
                StartCoroutine(waitForSceneToLoad(this.LoadingScreenName));
            }
            else if(!didInitLoadingScreen)
            {
                initLoadingScreen();
            }
            else if(!didStartLoading){
                StartCoroutine(LoadLevel());
                didStartLoading = true;
            }
            else{
                //put other loading screen scripts here (maybe display quotes or facts about the game)
            }
        }
    }

    //starts update script loop
    public void StartLoad(string sceneName, bool includeHUDtemp = true)
    {
        //loads loading screen scene
        didStartLoad = true;
        SceneManager.LoadScene(this.LoadingScreenName, LoadSceneMode.Single);
        sceneToLoad = sceneName;
        didLoadScene = false;
        includeHUD = includeHUDtemp;
    }

    //loads level specified within the loading screen scene
    private static IEnumerator LoadLevel()
    {
        bool won = ProgressManager.GetInstance().IsWon();

        //starts async load of scene specif ied
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        if(includeHUD)
        {
            SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
            SceneManager.LoadScene("Notifications", LoadSceneMode.Additive);
            SceneManager.LoadScene("Waypoints", LoadSceneMode.Additive);
        }
        //HUDLoad.allowSceneActivation = false;

        if (PlanetRoomManager.currentPlanet != null && sceneToLoad == "PlanetRoom" && PlanetRoomManager.currentPlanet.isBoss)
        {
            SceneManager.LoadScene("BossHealth", LoadSceneMode.Additive);
        }
        else if(PlanetRoomManager.currentPlanet != null && SceneManager.GetSceneByName("BossHealth").isLoaded)
        {
            SceneManager.UnloadSceneAsync("BossHealth");
        }

        asyncLoad.allowSceneActivation = false;

        //updates progressBar accordingly
        while(!asyncLoad.isDone)
        {
            progressBar.fillAmount = asyncLoad.progress;
            if(asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;


        }

        //reset values when done
        didLoadScene = true;
        didStartLoad = false;
        didInitLoadingScreen = false;
    }
    //waits until a scene has been loaded (in order to get the progress bar for loading)
    private IEnumerator waitForSceneToLoad(string sceneToLoad)
    {
        //waits until scene the correct one specified
        while(SceneManager.GetActiveScene().name != sceneToLoad)
        {
            yield return null;
        }
    }

    //initializes the loading screen
    private void initLoadingScreen()
    {
        //sets up variables used within the loading screen itself
        GameObject progress = GameObject.Find("Canvas/progressBar/progressBarFill");
        progressBar = progress.GetComponent<Image>();
        progressBar.fillAmount = 0;
        didInitLoadingScreen = true;
        didStartLoading = false;
    }
}
