using PlayerObject;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Upgrades;
using UnityEngine.InputSystem;

public class HUDInteractions : MonoBehaviour
{
	[SerializeField] public string GameScene;
	[SerializeField] public string StartScene;
	[SerializeField] public string HUDScene;
	[SerializeField] public PauseHandler PauseHandler;
	[SerializeField] public GameObject PauseMenuPanel;
	[SerializeField] public GameObject HUDPanel;
	[SerializeField] public GameObject GameOverScreen;
	[SerializeField] public GameObject WinScreen;
	[SerializeField] public GameObject UpgradesPanel;
	[SerializeField] public GameObject UpgradeList;
	[SerializeField] public GameObject UpgradeListTextObj;
	[SerializeField] public GameObject SettingsPanel;
	[SerializeField] public GameObject UpgradesButtonPanel;
	[SerializeField] public GameObject HealthUpgradesPanel;
	[SerializeField] public GameObject ShieldUpgradesPanel;
	[SerializeField] public GameObject DebugUpgradesPanel;
	[SerializeField] public GameObject WeaponUpgradesPanel;
	[SerializeField] public GameObject GameplayUpgradesPanel;
	[SerializeField] public GameObject ExitUpgradesButton;
	[SerializeField] public GameObject BackButton;
	[SerializeField] public GameObject LaserWeaponSprite;
	[SerializeField] public GameObject RocketWeaponSprite;
	[SerializeField] public TMP_Text HUDCurrencyText;
	[SerializeField] public TMP_Text UpgradesCurrencyText;
	[SerializeField] public TMP_Text NumLivesText;
	[SerializeField] public UnityEngine.UI.Slider HealthBarSlider;
	[SerializeField] public UnityEngine.UI.Slider AmmoSlider;
	[SerializeField] public UnityEngine.UI.Slider MasterVolSlider;
	[SerializeField] public UnityEngine.UI.Slider MusicVolSlider;
	[SerializeField] public UnityEngine.UI.Slider SFXVolSlider;
	private IUpgradeInventory inventory;
	private IPlayer player;
	private ProgressManager Progress;
	private AudioManager _audioManager;
	private InputActions InputActions;
	private InputAction QuitAction;

	private SceneLoader loader;
	private bool onlyShowOnce = false;

	#region Unity Methods

	private void OnEnable()
	{
		player = PlayerManager.players[0];
		inventory = player.inventory;
		AmmoSlider.maxValue = player.rocketAmmo;
		//PlayerShip.GameOverEvent += PlayerShip_GameOverEvent;
		Progress = new ProgressManager();
		_audioManager = FindAnyObjectByType<AudioManager>();
		MasterVolSlider.value = _audioManager.MasterVolume;
		MusicVolSlider.value = _audioManager.MusicVolume;
		SFXVolSlider.value = _audioManager.SFXVolume;
		InputActions = InputManager.GetInputActions();
		QuitAction = InputActions.UI.Quit;
		QuitAction.performed += QuitAction_Performed;
		QuitAction.Enable();
		GameObject temp = GameObject.Find("SceneLoader");
		if(temp == null)
        {
            GameObject tempObj = new GameObject("SceneLoader");
            loader = tempObj.AddComponent<SceneLoader>();
        }
        else
        {
            loader = temp.GetComponent<SceneLoader>();
        }
		onlyShowOnce = false;

		LoadPlayerUpgradeList();
	}

	private void OnDisable()
	{
		QuitAction.Disable();
	}

	private void LoadPlayerUpgradeList()
	{
		foreach(IUpgrade upgrade in player.inventory.upgrades)
		{
			AddUpgradeToList(upgrade.name);
		}
	}

	private void Update()
	{
		HUDCurrencyText.text = $"$: {CurrencyManager.GetCurrency(player)}";
		UpgradesCurrencyText.text = $"$: {CurrencyManager.GetCurrency(player)}";
		NumLivesText.text = $"x {player.lives}";
		HealthBarSlider.maxValue = player.Health;
		HealthBarSlider.value = player.currentHealth;
		AmmoSlider.value = player.projectiles.GetCurrentGun().ammo;
		UpdateCurrentWeaponSprite();

		//update settings when paused

		// I'd rather do this with events but that wasn't working and this is so ¯\_(ツ)_/¯
		if (player.lives <= 0 && !onlyShowOnce)
		{
			ShowGameOverScreen();
			onlyShowOnce = true;
		}
		else if (Progress.IsWon())
		{
            ShowWinScreen();
            Progress.setWin(false);
        }
	}

	#endregion

	#region Events

	private void QuitAction_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
	{
		// if upgrades menu open, close it, otherwise, toggle the pause menu
		if (UpgradesPanel.activeSelf)
		{
			ToggleUpgradesMenu();
			PauseHandler.TogglePauseGame();
		}
		else if(SettingsPanel.activeSelf)
		{
			ToggleSettingsMenu();
		}
		else
		{
			TogglePauseMenu();
			PauseHandler.TogglePauseGame();
		}
	}

	private void PlayerShip_GameOverEvent(object sender, EventArgs e)
	{
		ShowGameOverScreen();
	}
	

	#endregion

	#region Helper Methods

	private void UpdateCurrentWeaponSprite()
	{
		Type currentGunType = player.projectiles.GetCurrentGun().GetType();

		if (currentGunType == typeof(Lazer))
		{
			// laser
			DisableAllWeaponSprites();
			LaserWeaponSprite.SetActive(true);
		}
		else if (currentGunType == typeof(Rocket))
		{
			// rocket
			DisableAllWeaponSprites();
			RocketWeaponSprite.SetActive(true);
			AmmoSlider.gameObject.SetActive(true);
			AmmoSlider.maxValue = player.maxRocketAmmo;
		}
		else if (currentGunType == typeof(Beam))
		{
			// beam
			DisableAllWeaponSprites();
			LaserWeaponSprite.SetActive(true);
			AmmoSlider.gameObject.SetActive(true);
			AmmoSlider.maxValue = player.maxBeamAmmo;
		}
	}

	public void AddUpgradeToList(string upgradeName)
	{
		var textObj = Instantiate(UpgradeListTextObj);
		textObj.transform.parent = UpgradeList.transform;
		TextMeshProUGUI textCom = textObj .GetComponent<TextMeshProUGUI>();
		textCom.text = upgradeName;
		textObj.SetActive(true);
	}

	public void RemoveUpgradeFromList(string upgradeName)
	{
		//not implemented yet :)
	}

	private void DisableAllWeaponSprites()
	{
		LaserWeaponSprite.SetActive(false);
		RocketWeaponSprite.SetActive(false);
		AmmoSlider.gameObject.SetActive(false);
	}

	private void ShowGameOverScreen()
	{
		PauseMenuPanel.SetActive(false);
		HUDPanel.SetActive(false);
		UpgradesPanel.SetActive(false);
		GameOverScreen.SetActive(true);
		PauseHandler.PauseGame();
	}

    private void ShowWinScreen()
    {
        PauseMenuPanel.SetActive(false);
        HUDPanel.SetActive(false);
        UpgradesPanel.SetActive(false);
        WinScreen.SetActive(true);
    }

    private void TogglePauseMenu()
	{
		// disable pause menu and reenable HUD
		PauseMenuPanel.SetActive(!PauseMenuPanel.activeSelf);
		HUDPanel.SetActive(!HUDPanel.activeSelf);

	}

	private void ToggleSettingsMenu()
	{
		// disable pause menu and reenable HUD
		PauseMenuPanel.SetActive(!PauseMenuPanel.activeSelf);
		SettingsPanel.SetActive(!PauseMenuPanel.activeSelf);
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

	private void ToggleUpgradesMenu()
	{
		UpgradesPanel.SetActive(!UpgradesPanel.activeSelf);
		HUDPanel.SetActive(!HUDPanel.activeSelf);
		ResetUpgradesMenu();
		PauseMenuPanel.SetActive(false);
	}

	private void ResetUpgradesMenu()
	{
		// Disable any upgrade submenus, and reenable the main upgrade menu
		HealthUpgradesPanel.SetActive(false);
		ShieldUpgradesPanel.SetActive(false);
		WeaponUpgradesPanel.SetActive(false);
		GameplayUpgradesPanel.SetActive(false);
		DebugUpgradesPanel.SetActive(false);
		UpgradesButtonPanel.SetActive(true);
	}

	private void ToggleBackButton()
	{
		BackButton.SetActive(!BackButton.activeSelf);
		ExitUpgradesButton.SetActive(!ExitUpgradesButton.activeSelf);
	}

	#endregion

	#region Button OnClick Handlers

	public void PauseButton_OnClick()
	{
		TogglePauseMenu();
		PauseHandler.TogglePauseGame();
	}
	

	public void ExitButton_OnClick()
	{
		TogglePauseMenu();
	}

	public void ContinueButton_OnClick()
	{
		TogglePauseMenu();
		PauseHandler.TogglePauseGame();
	}

	public void SettingsButton_OnClick()
	{
		ToggleSettingsMenu();
	}

	public void QuitButton_OnClick()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		//SceneManager.UnloadSceneAsync(GameScene);
		//SceneManager.UnloadSceneAsync(HUDScene);
		Application.Quit();
	}

	public void ResetButton_OnClick()
	{
		// potential issue with the reset game being paused on reset, causing sound issues
		//SceneManager.UnloadSceneAsync(GameScene);
		PauseHandler.UnpauseGame();
		PlanetRoomManager.currentPlanet = null;
		loader.StartLoad(StartScene, false);
		//regenerate graph?
	}

	public void UpgradesButton_OnClick()
	{
		ToggleUpgradesMenu();
		PauseHandler.TogglePauseGame();
	}

	public void ExitUpgradesButton_OnClick()
	{
		ToggleUpgradesMenu();
		PauseHandler.TogglePauseGame();
	}

	public void BackButton_OnClick()
	{
		ToggleBackButton();
		ResetUpgradesMenu();
	}

	public void HealthUpgradesButton_OnClick()
	{
		HealthUpgradesPanel.SetActive(true);
		UpgradesButtonPanel.SetActive(false);
		ToggleBackButton();
	}

	public void ShieldUpgradesButton_OnClick()
	{
		ShieldUpgradesPanel.SetActive(true);
		UpgradesButtonPanel.SetActive(false);
		ToggleBackButton();
	}

	public void DebugUpgradesButton_OnClick()
	{
		DebugUpgradesPanel.SetActive(true);
		UpgradesButtonPanel.SetActive(false);
		ToggleBackButton();
	}

	public void WeaponUpgradesButton_OnClick()
	{
		WeaponUpgradesPanel.SetActive(true);
		UpgradesButtonPanel.SetActive(false);
		ToggleBackButton();
	}

	public void GameplayUpgradesButton_OnClick()
	{
		GameplayUpgradesPanel.SetActive(true);
		UpgradesButtonPanel.SetActive(false);
		ToggleBackButton();
	}

	#endregion

	#region Upgrade OnClick Handlers

	#region Health Upgrades

	public void ExtraLifeButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 1000))
		{
			inventory.AddUpgrade(new ExtraLife());
			_audioManager.PlayItemPurchased();
		}
	}

	public void IncreaseHealthCapButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 200))
		{
			inventory.AddUpgrade(new IncreaseHealthCap());
			_audioManager.PlayItemPurchased();
		}
	}

	public void ReplenishHealthButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 100))
		{
			inventory.AddUpgrade(new ReplenishPlayerHealth());
			_audioManager.PlayItemPurchased();
		}
	}

	#endregion

	#region Weapon Upgrades

	public void IncreaseFireRateButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 250))
		{
			inventory.AddUpgrade(new IncreaseFireRate());
			_audioManager.PlayItemPurchased();
		}
		else
		{
			// throw error by playing sound, popup, etc???
		}
	}

	public void IncreaseShotSpeedButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 200))
		{
			inventory.AddUpgrade(new IncreaseShotSpeed());
			_audioManager.PlayItemPurchased();
		}

		// same as above

	}

	public void DamageIncreaseButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 300))
		{
			inventory.AddUpgrade(new IncreaseDamagePercent());
			_audioManager.PlayItemPurchased();
		}
	}

	public void ShotSizeIncreaseButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 400))
		{
			inventory.AddUpgrade(new IncreaseProjectileSize());
			_audioManager.PlayItemPurchased();
		}
	}

	public void RefillRocketAmmoButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 300))
		{
			inventory.AddUpgrade(new RefillRocketAmmo());
			_audioManager.PlayItemPurchased();
		}
	}

	#endregion

	#region Gameplay Upgrades

	public void PlayerSizeIncreaseButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 300))
		{
			inventory.AddUpgrade(new IncreasePlayerSize());
			_audioManager.PlayItemPurchased();
		}
	}

	public void IncreaseMovementButton_OnClick()
	{
		if (CurrencyManager.SpendCurrency(player, 400))
		{
			inventory.AddUpgrade(new IncreaseMovement());
			_audioManager.PlayItemPurchased();
		}
	}

	#endregion

	#region Debug Upgrades

	// Debug Upgrades -----------------------------------------------
	// Just for debugging purposes for TB5, not checking for currency for these

	public void IncreaseShotFanButton_OnClick()
	{
		// if (CurrencyManager.SpendCurrency(player, 300))
		// {
		inventory.AddUpgrade(new IncreaseShotFan());
		_audioManager.PlayItemPurchased();
		//}
		//...
	}

	public void AddShotButton_OnClick()
	{
		// if (CurrencyManager.SpendCurrency(player, 400))
		// {
		inventory.AddUpgrade(new AddShot());
		_audioManager.PlayItemPurchased();
		//}
		//...
	}

	public void FlameShot_OnClick()
	{
		inventory.AddUpgrade(new FlameShot());
		_audioManager.PlayItemPurchased();
	}

	public void GravityShot_OnClick()
	{
		inventory.AddUpgrade(new Upgrades.GravityShot());
		_audioManager.PlayItemPurchased();
	}

	public void PiercingShot_OnClick()
	{
		inventory.AddUpgrade(new PiercingShot());
		_audioManager.PlayItemPurchased();
	}

	public void SpectralShot_OnClick()
	{
		inventory.AddUpgrade(new SpectralShot());
		_audioManager.PlayItemPurchased();
	}

	public void LightningShot_OnClick()
	{
		inventory.AddUpgrade(new Upgrades.LightningShot());
		_audioManager.PlayItemPurchased();
	}

	public void ControlledShot_OnClick()
	{
		inventory.AddUpgrade(new ControlledShot());
		_audioManager.PlayItemPurchased();
	}

	public void Polyphemus_OnClick()
	{
		inventory.AddUpgrade(new Polyphemus());
		_audioManager.PlayItemPurchased();
	}
	#endregion

	#endregion
}
