using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Audio manager idea and code from: https://medium.com/@cwagoner78/creating-an-organized-sound-system-and-playing-sounds-in-unity-82cbb48060ff
public class AudioManager : MonoBehaviour
{
    //public static AudioManager Instance {get;} = new AudioManager();

    private List<AudioSource> musicClips;
    private List<AudioSource> sfxClips;

    [Header("Music")]
    [SerializeField] public AudioSource _titleMusic;
    [SerializeField] public AudioSource _gameMusic;
    [SerializeField] public AudioSource _combatMusicSource;
    [SerializeField] public AudioClip[] _combatMusicClips;
    [SerializeField] public AudioSource _pauseMusic;
    [SerializeField] private float _fadeTimerInterval = 0.001f;
    private bool _titleMusicPlaying = false;
    private bool _gameMusicPlaying = false;
    private bool _combatMusicPlaying = false;
    private bool _pauseMusicPlaying = false;

    [Header("Player")]
    [SerializeField] public AudioSource _engineSource;
    [SerializeField] public AudioSource _hyperspaceSource;
    [SerializeField] public AudioSource _shieldBreakSource;
    [SerializeField] public AudioSource _shieldRechargeSource;
    [SerializeField] public AudioSource _shieldDamageSource;
    /*
        [Header("Explosions")]
        [SerializeField] private AudioSource _explosionSource;
        [SerializeField] private AudioClip[] _explosionClips;*/

    [Header("Projectiles")]
    [SerializeField] private AudioSource _lazerProjectileSource;
    [SerializeField] private AudioClip[] _lazerProjectileClips;
    [SerializeField] private AudioSource _explosionSource;
    [SerializeField] private AudioClip[] _explosionClips;
    [SerializeField] private AudioSource _missileFireSource;
    [SerializeField] private AudioClip[] _missileFireClips;
    [SerializeField] private AudioSource _shieldBallisticDamageSource;

    [Header("Items")]
    [SerializeField] public AudioSource _itemPickupSource;
    [SerializeField] public AudioSource _itemPurchased;
    [SerializeField] public AudioClip[] _itemPurchasedClips;

    [Header("UI")]
    [SerializeField] public AudioSource _pause;
    [SerializeField] public AudioSource _unPause;

    //in the future, load these values from a json file or another storage file
    [Header("Settings")]
    [SerializeField] public float MasterVolume = .5f;
    [SerializeField] public float MusicVolume = .5f;
    [SerializeField] public float SFXVolume = .5f;

    void Awake()
    {
        //checks if object is not in dont destroy on load already
        if (this.gameObject.scene.buildIndex != -1)
        {
            //if object isnt the only audio manager, then destroy it
            if(GameObject.FindGameObjectsWithTag("Audio").Length == 1)
            {
                DontDestroyOnLoad(this.gameObject);
                //read in settings values and set accordingly
                CreateLists();
                UpdateMasterVolume(MasterVolume);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        {
            CreateLists();
            UpdateMasterVolume(MasterVolume);
        }
    }

    //constructor only inits once
    private AudioManager()
    {
        //CreateLists();
        musicClips = new List<AudioSource>();
        sfxClips = new List<AudioSource>();

        //if (instance == null) instance = this;
        //else Destroy(gameObject);
    }

    public void StartTitleMusic()
    {
        if(!_titleMusic.isPlaying)
        {
            _titleMusic.Play();
            _gameMusic.Stop();
            _gameMusicPlaying = false;
            _titleMusicPlaying = true;
        }
         
        // if(_gameMusicPlaying)
        // {
        //     //_gameMusic.Stop();
        //     _gameMusicPlaying = false;
        // }
    }

    private void StartMusic(AudioSource musicTrack)
    {
        foreach(AudioSource music in musicClips)
        {
            if(music != null)
            {
                if(music.clip == musicTrack.clip)
                {
                    if(!music.isPlaying)
                    {
                        music.Play();
                    }
                }
                // else
                // {
                //     if(music.isPlaying)
                //     {
                //         music.Pause();
                //     }
                // }
            }
        }
    }

    private IEnumerator FadeMusic(AudioSource musicTrack, bool fadeOut)
    {
        if (fadeOut)
        {
            Debug.Log("music fading out.");
            musicTrack.volume = .015f;
            while (musicTrack.volume > 0)
            {
                musicTrack.volume -= 0.001f;
                yield return new WaitForSeconds(_fadeTimerInterval);
            }
            musicTrack.Stop();
        }
        else if (!fadeOut)
        {
            Debug.Log("music fading in.");
            musicTrack.Play();
            musicTrack.volume = 0;
            while (musicTrack.volume < MasterVolume * MusicVolume)
            {
                musicTrack.volume += 0.001f;
                yield return new WaitForSeconds(_fadeTimerInterval);
            }
        }
    }

    private void StopMusic(AudioSource musicTrack)
    {
        foreach(AudioSource music in musicClips)
        {
            if(music != null)
            {
                if(music.clip == musicTrack.clip)
                {
                    if(music.isPlaying)
                    {
                        music.Stop();
                    }
                }
                // else
                // {
                //     if(music.isPlaying)
                //     {
                //         music.Pause();
                //     }
                // }
            }
        }
    }

    public void StopAllMusic()
    {
        foreach(AudioSource music in musicClips)
        {
            if(music != null && music.isPlaying)
            {
                music.Stop();
            }
        }
    }

    public void StartGameMusic()
    {
        if (!_gameMusic.isPlaying)
        {
            _gameMusic.Play();
           _titleMusic.Stop();
            _titleMusicPlaying = false;
            _gameMusicPlaying = true;
        }
    }

    public void StartCombatMusic()
    {
        if (!_combatMusicSource.isPlaying)
        {
            _combatMusicSource.Play();
            _gameMusic.Stop();
            _gameMusicPlaying = false;
            _combatMusicPlaying = true;
        }
    }

    public void StartPauseMusic()
    {
        if (!_pauseMusic.isPlaying)
        {
            _pauseMusic.Play();
            _pauseMusicPlaying = true;
        }
    }

    public void StopCombatMusic()
    {
        if(_combatMusicSource.isPlaying)
        {
            _combatMusicSource.Stop();
            _combatMusicPlaying = false;
        }
    }

    public void GamePaused(bool paused)
    {
        if (paused)
        {
            _pauseMusic.Play();
            _gameMusic.Pause();
            _combatMusicSource.Pause();
            _engineSource.Pause();
        }
        if (!paused)
        {
            _pauseMusic.Stop();
            _gameMusic.UnPause();
            _combatMusicSource.UnPause();
            _engineSource.Play();
        }
    }

    public void StartFadeTitleMusicOut()
    {
        StartCoroutine(FadeTitleMusic(true));
    }

    private void StartFadeTitleMusicIn()
    {
        StartCoroutine(FadeTitleMusic(false));
    }

    public void StartFadeGameMusicOut()
    {
        StartCoroutine(FadeOutGameMusic(true));
    }

    public void StartFadeGameMusicIn()
    {
        StartCoroutine(FadeOutGameMusic(false));
    }

    public void StartFadeCombatMusicOut()
    {
        StartCoroutine(FadeOutCombatMusic(true));
    }

    public void StartFadeCombatMusicIn()
    {
        StartCoroutine(FadeOutCombatMusic(false));
    }

    public void FadeRandomCombatMusicIn()
    {
        int clip = Random.Range(0, _combatMusicClips.Length);
        _combatMusicSource.clip = _combatMusicClips[clip];
        StartCoroutine(FadeMusic(_combatMusicSource, false));
    }

    private IEnumerator FadeTitleMusic(bool fadeOut)
    {
        return FadeMusic(_titleMusic, fadeOut);
    }

    private IEnumerator FadeOutGameMusic(bool fadeOut)
    {
        return FadeMusic(_gameMusic, fadeOut);
    }

    private IEnumerator FadeOutCombatMusic(bool fadeOut)
    {
        return FadeMusic(_combatMusicSource, fadeOut);
    }

    public void PlayRandomLazerProjectile()
    {
        int clip = Random.Range(0, _lazerProjectileClips.Length);
        _lazerProjectileSource.PlayOneShot(_lazerProjectileClips[clip]);


    }

    public void PlayRandomCombatMusic()
    {
        int clip = Random.Range(0, _combatMusicClips.Length);
        _combatMusicSource.clip = _combatMusicClips[clip];
        _combatMusicSource.Play();
    }

    public void PlayRandomExplosion()
    {
        int clip = Random.Range(0, _explosionClips.Length);
        _explosionSource.PlayOneShot(_explosionClips[clip]);

    }

    public void PlayRandomMissileFire()
    {
        if(_missileFireClips.Length > 0)
        {
            int clip = Random.Range(0, _missileFireClips.Length);
            _missileFireSource.PlayOneShot(_missileFireClips[clip]);
        }

    }

    public void PlayBeamCharge(float timeToCompleteCharge)
    {
        _shieldRechargeSource.volume = MasterVolume * SFXVolume * 3;
        //seeks to time closer to end if fire rate is higher
        float seekTime = _shieldRechargeSource.clip.length - timeToCompleteCharge;
        if(seekTime < 0)
        {
            seekTime = 0;
        }
        _shieldRechargeSource.time = seekTime;
        //_shieldRechargeSource.volume = 0.015f;
        _shieldRechargeSource.Play();

    }

    public void StopBeamCharge()
    {
        _shieldRechargeSource.Stop();

    }
    

    public void PlayBeamShoot()
    {
        _shieldBreakSource.volume = MasterVolume * SFXVolume * 3;
        _shieldBreakSource.Play();
    }

    public void PlayItemPurchased()
    {
        int clip = Random.Range(0, _itemPurchasedClips.Length);
        _itemPurchased.PlayOneShot(_itemPurchasedClips[clip]);
    }

    public void PlayItemPickup()
    {
        _itemPickupSource.Play();
    }

    public void UpdateMasterVolume(float val)
    {
        MasterVolume = val;
        float newMusicVol = MasterVolume * MusicVolume;
        float newSFXVol = MasterVolume * SFXVolume;

        UpdateClips(newMusicVol, musicClips);

        UpdateClips(newSFXVol, sfxClips);

        UpdateCertainClips();
    }

    public void UpdateMusicVolume(float val)
    {
        MusicVolume = val;
        float newVolume = MasterVolume * MusicVolume;
        UpdateClips(newVolume, musicClips);

        UpdateCertainClips();
    }

    public void UpdateSFXVolume(float val)
    {
        SFXVolume = val;
        float newVolume = MasterVolume * SFXVolume;

        UpdateClips(newVolume, sfxClips);

        UpdateCertainClips();
    }

    private void UpdateClips(float volume, List<AudioSource> clips)
    {
       foreach(AudioSource audio in clips)
        {
            if(audio != null) 
            {
                audio.volume = volume;
            }
        } 
    }

    private void UpdateCertainClips()
    {
        //_combatMusicSource.volume = MasterVolume * MusicVolume / 2;
        _explosionSource.volume = MasterVolume * SFXVolume / 2;
        _missileFireSource.volume = MasterVolume * SFXVolume / 2;
    }

    private void CreateLists()
    {
        musicClips.Add(_titleMusic);
        musicClips.Add(_gameMusic);
        musicClips.Add(_combatMusicSource);
        musicClips.Add(_pauseMusic);
        sfxClips.Add(_engineSource);
        sfxClips.Add(_hyperspaceSource);
        sfxClips.Add(_shieldBreakSource);
        sfxClips.Add(_shieldRechargeSource);
        sfxClips.Add(_shieldDamageSource);
        sfxClips.Add(_lazerProjectileSource);
        sfxClips.Add(_itemPurchased);
        sfxClips.Add(_shieldBallisticDamageSource);
        sfxClips.Add(_itemPickupSource);
        sfxClips.Add(_explosionSource);
        sfxClips.Add(_missileFireSource);
    }

}