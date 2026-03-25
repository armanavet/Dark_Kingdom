using AudioSystem;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SoundData music;
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
        music = AudioManager.Instance.SetData(SoundDataType.Music);
        UIManager.Instance.Initialize();
        PortalManager.Instance.Initialize();
        TowerManager.Instance.Initialize();
        WaveManager.Instance.Initialize();
        StateManager.Instance.Initialize();

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
        if (music == null)
        {
            Debug.LogError("Music data is null");
            return;
        }

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
        if (AudioManager.Instance == null || music == null)
        {
            Debug.LogWarning("I got null");
            yield break;
        }
        if (state == GameState.Passive)
        {
            AudioManager.Instance.Play(MusicType.InNormal, music, transform);
            Debug.Log(music.clip);
        }
        else if (state == GameState.Active)
        {
            //music.clip = null;
            AudioManager.Instance.Play(GamePlaySFX_Type.WaveStart, music, transform);
            yield return new WaitForSeconds(music.clip.length);
            OnWaveIntroFinished?.Invoke();
            AudioManager.Instance.Play(MusicType.InWave, music, transform);
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
