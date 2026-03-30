using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] SoundData StateSoundData;
    [SerializeField] float[] TimeUntilNextWave;
    [HideInInspector] public GameState State;
    GameState PreviusState;
    float Timer;
    public int timeMultiplier = 1;
    public event Action<GameState> OnGameStateChanged;
    #region Singleton
    private static StateManager _instance;
    public static StateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StateManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public void Initialize()
    {
        ChangeGameStateTo(GameState.Passive);
    }

    void Update()
    {
        Time.timeScale = timeMultiplier;
        if (State == GameState.Passive)
        {
            Timer -= Time.deltaTime * timeMultiplier;
            UIManager.Instance.GameTimer = Timer;
            if (Timer <= 0) ChangeGameStateTo(GameState.Active);
        }
        else if (State == GameState.End)
        {
            //UIManager.Instance.ShowEndGamePanel();
        }
    }
    public void ChangeGameStateTo(GameState newState)
    {
        if (newState == GameState.Active)
        {
            State = GameState.Active;
            PreviusState = State;
            timeMultiplier = 1;
        }
        else if (newState == GameState.Passive)
        {
            State = GameState.Passive;
            PreviusState = State;
            Timer = (WaveManager.Instance.CurrentWaveIndex <= TimeUntilNextWave.Length) ? TimeUntilNextWave[WaveManager.Instance.CurrentWaveIndex] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
            SaveManager.Save();
        }
        else if (newState == GameState.Paused)
        {
            State = GameState.Paused;
            PauseMenuManager.Instance.ShowPauseMenu(true);
        }
        else if (newState == GameState.Resume)
        {
            State = PreviusState;
            PauseMenuManager.Instance.ShowPauseMenu(false);
        }
        else if (newState == GameState.End)
        {
            State = GameState.End;
        }
        OnGameStateChanged?.Invoke(newState);
        UIManager.Instance.OnGameStateChanged();
    }
    public void ButtonToMakeFaster(int multiplier)
    {
        timeMultiplier = (timeMultiplier == multiplier) ? (timeMultiplier = 1) : (timeMultiplier = multiplier);
    }

    //public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    //public string GetUniqueSaveID()
    //{
    //    return nameof(StateManager);
    //}

    //public ISaveData SaveState()
    //{
    //    GeneralData saveData = new GeneralData();

    //    return saveData;
    //}

    //public void LoadState(ISaveData data)
    //{
    //    GeneralData saveData = data as GeneralData;
    //}
}

public enum GameState
{
    Active,
    Passive,
    Paused,
    Resume,
    End
}