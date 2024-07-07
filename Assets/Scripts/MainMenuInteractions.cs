using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MainMenuInteractions : MonoBehaviour
{
	[SerializeField]
	public string SceneToLoadOnPlay;
	[SerializeField]
	public int numGraphEdges = 0;
	[SerializeField]
	public int numGraphLevels = 0;

	[SerializeField]
	public int startingMoney = 100;

	[SerializeField]
	public int numUiGraphRows = 2;
	[SerializeField]
	public float UiGraphXOffset = 20f;
	[SerializeField]
	public float UiGraphYOffset = 20f;
	[SerializeField]
	public int graphGenerationSeed = 123123;

	[SerializeField]
	public Sprite CircleSprite;
	[SerializeField] public GameObject MainMenuPanel;
	[SerializeField] public GameObject SettingsMenuPanel;

	[SerializeField]
	public Sprite CurrentLevelSprite;

	[SerializeField] private SceneLoader SceneLoader;
    [SerializeField] public int MinPlanets = 6;
    [SerializeField] public int MaxPlanets = 10;
	[SerializeField] public List<GameObject> BossPrefabs;
    [SerializeField] public List<GameObject> PlanetPrefabs;
	[SerializeField] public List<GameObject> PlayerPrefabs;
	[SerializeField] public List<GameObject> ProjectilePrefabs;
    [SerializeField] public List<GameObject> SunPrefabs;
	[SerializeField] public List<GameObject> AsteroidPrefabs;
    [SerializeField] public GameObject Player;

	[SerializeField] public UnityEngine.UI.Slider MasterVolSlider;
	[SerializeField] public UnityEngine.UI.Slider MusicVolSlider;
	[SerializeField] public UnityEngine.UI.Slider SFXVolSlider;

	private AudioManager _audioManager;

	//input params for PlayerLevelManager
	void Start()
	{
		//initialize all static classes
		SceneLoader.Initialize();
		PlayerManager.Initialize(PlayerPrefabs, ProjectilePrefabs);
		GameObject mapUI = GameObject.Find("MapCanvas");
		UpgradeManager.Initialize();
		PlayerLevelManager.Initialize(SceneLoader, SceneToLoadOnPlay, CircleSprite, CurrentLevelSprite, mapUI, MinPlanets, MaxPlanets, PlanetPrefabs, SunPrefabs, BossPrefabs, AsteroidPrefabs);
		PlayerLevelManager.toggleUIGraph();
		_audioManager = FindObjectOfType<AudioManager>();
		_audioManager.StopAllMusic();
		_audioManager.StartTitleMusic();

		//set values for settings
		MasterVolSlider.value = _audioManager.MasterVolume;
		MusicVolSlider.value = _audioManager.MusicVolume;
		SFXVolSlider.value = _audioManager.SFXVolume;

		if(!PlanetRoomManager.didInit)
        {
            PlanetRoomManager.Initialize(-1);
            PlanetRoomManager.SetPlanetPrefabs(PlanetPrefabs, SunPrefabs, BossPrefabs, AsteroidPrefabs);
			
        }
		 
	}
	public void PlayButton_OnClick()
	{
		//graphGenerationSeed = new System.Random().Next();
		PlayerManager.AddNewPlayer();
		PlayerLevelManager.createGraph(numGraphLevels, numGraphEdges, numUiGraphRows, UiGraphXOffset, UiGraphYOffset, graphGenerationSeed);
		PlayerLevelManager.toggleUIGraph();
		CurrencyManager.Initialize(startingMoney);
		PlayerLevelManager.start();
	}

	public void QuitButton_OnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	public void SettingsButton_OnClick()
	{
		SettingsMenuPanel.SetActive(true);
		MainMenuPanel.SetActive(false);
	}

	public void CloseSettingsButton_OnClick()
	{
		SettingsMenuPanel.SetActive(false);
		MainMenuPanel.SetActive(true);
	}

	public void ChangeMasterVolume(float val)
	{
		_audioManager.UpdateMasterVolume(val);
	}

	public void ChangeMusicVolume(float val)
	{
		_audioManager.UpdateMusicVolume(val);
	}

	public void ChangeSFXVolume(float val)
	{
		_audioManager.UpdateSFXVolume(val);
	}
}
