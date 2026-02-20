using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour, ISaveable
{
    [SerializeField] SoundData StateSoundData;
    [SerializeField] float[] TimeUntilNextWave;
    [HideInInspector] public GameState State;
    GameState PreviusState;
    float Timer;
    public int timeMultiplier = 1;
    int currentWave = 0;
    public event Action<GameState> OnGameStateChanged;
    #region Singleton
    private static StateManager _instance;
    public static StateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<StateManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        RegisterSaveable();
    }
    #endregion

    public void Initialize()
    {
        //StateSoundData = AudioManager.Instance.SetData(StateSoundData,SoundDataType.Music, MixerType.State);
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
        //AudioManager.Instance.Play(StateSoundData, newState);
        int wave = currentWave;
        if (newState == GameState.Active)
        {
            Debug.Log("enter the active phase " + currentWave + " times.");
            State = GameState.Active;
            PreviusState = State;
            //timeMultiplier = 1;
            WaveManager.Instance.StartSpawn();
        }
        else if (newState == GameState.Passive)
        {
            State = GameState.Passive;
            PreviusState = State;
            Timer = (currentWave <= TimeUntilNextWave.Length) ? TimeUntilNextWave[wave] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
            if (wave == WaveManager.Instance.waveLength - 1) WaveManager.Instance.GetPhaseCommands(wave, true);
            else WaveManager.Instance.GetPhaseCommands(wave);
            currentWave++;
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
        UIManager.Instance.OnGameStateChanged(currentWave);
    }
    public void ButtonToMakeFaster(int multiplier)
    {
        timeMultiplier = (timeMultiplier == multiplier) ? (timeMultiplier = 1) : (timeMultiplier = multiplier);
    }

    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    public string GetUniqueSaveID()
    {
        return nameof(StateManager);
    }

    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.CurrentWave = currentWave;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentWave = saveData.CurrentWave;
    }
}

public enum GameState
{
    Active,
    Passive,
    Paused,
    Resume,
    End
}