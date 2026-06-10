using AudioSystem;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    AudioSource music;
    static bool IsPaused;
    Coroutine musicRoutine;
    public event Action OnWaveIntroFinished;
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    void Start()
    {
        AudioManager.Instance.Initialize();
        music = GetComponent<AudioSource>();
        if (music == null)
        {
            Debug.LogError($"The {this} has no audio source!");
        }
        UIManager.Instance.Initialize();
        PortalManager.Instance.Initialize();
        TowerManager.Instance.Initialize();
        WaveManager.Instance.Initialize();
        StateManager.Instance.Initialize();
        StrategyManager.Instance.Initialize();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            PauseGame();
        }
    }
    public void PlayMusic(GameState state)
    {
        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
            musicRoutine = null;
        }
        musicRoutine = StartCoroutine(ChooseTheMusic(state));
    }
    public void PauseGame()
    {
        if (IsPaused)
        {
            Time.timeScale = 0f;
            StateManager.Instance.ChangeGameStateTo(GameState.Paused);
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            StateManager.Instance.ChangeGameStateTo(GameState.Resume);
            AudioListener.pause = false;
        }
    }
    IEnumerator ChooseTheMusic(GameState state)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning($"The {AudioManager.Instance} is null");
            yield break;
        }
        if (state == GameState.Passive)
        {
            yield return new WaitForSeconds(1f);
            AudioManager.Instance.Play(MusicType.InNormal, music);
        }
        else if (state == GameState.Active)
        {
            AudioManager.Instance.Play(GamePlaySFX_Type.WaveStart, music);
            yield return new WaitForSeconds(music.clip.length);
            OnWaveIntroFinished?.Invoke();
            AudioManager.Instance.Play(MusicType.InWave, music);
        }
    }
    void OnEnable()
    {
        StateManager.Instance.OnGameStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        if (StateManager.Instance != null)
            StateManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }
    void HandleStateChanged(GameState state)
    {
        PlayMusic(state);
    }
}
