using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public static bool GameIsPaused;

    private AudioManager _audioManager;

    public void Start()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void OnEnable()
	{
		GameIsPaused = false;
	}

    /// <summary>
    /// Change the state of the game from paused to unpaused, or unpaused to paused.
    /// </summary>
	public void TogglePauseGame()
    {
        if (GameIsPaused)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Pause the game if it is currently playing, or do nothing if it is already paused.
    /// </summary>
    public void PauseGame()
    {
		Time.timeScale = 0f;
		_audioManager.GamePaused(true);
        //_audioManager.StartMusic(_audioManager._pauseMusic);
        GameIsPaused = true;
	}

    /// <summary>
    /// Unpause the game if it is currently paused, or do nothing if it is already playing.
    /// </summary>
    public void UnpauseGame()
    {
		Time.timeScale = 1.0f;
		_audioManager.GamePaused(false);
        //_audioManager.StartMusic(_audioManager._combatMusic);
		GameIsPaused = false;
	}
    public bool IsPaused()
    {
        return GameIsPaused;
    }
}
