using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour, ISaveable
{

    [SerializeField] float[] TimeUntilNextWave;
    [HideInInspector] public GameState State;
    float Timer;
    public int timeMultiplier = 1;
    int currentWave = 0;

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
        ChangeGameStateTo(GameState.Passive);
    }

    void Update()
    {
        if (State == GameState.Passive)
        {
            Timer -= Time.deltaTime * timeMultiplier;
            UIManager.Instance.GameTimer = Timer;
            if (Timer <= 0) ChangeGameStateTo(GameState.Active);
        }
        else if (State == GameState.End)
        {
            UIManager.Instance.ShowEndGamePanel();
        }
    }
    public void ChangeGameStateTo(GameState newState)
    {
        int wave = currentWave;
        if (newState == GameState.Active)
        {
            State = GameState.Active;
            timeMultiplier = 1;
            AudioManager.Instance.PlayBackgroundMusic(newState);
            WaveManager.Instance.StartSpawn();
        }
        else if (newState == GameState.Passive)
        {
            //if (wave < WaveManager.Instance.waveLength)
            //{
            // this if is not necessary 
            State = GameState.Passive;
            Timer = (currentWave <= TimeUntilNextWave.Length) ? TimeUntilNextWave[wave] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
            AudioManager.Instance.PlayBackgroundMusic(newState);
            if (wave == WaveManager.Instance.waveLength - 1) WaveManager.Instance.GetPhaseCommands(wave, true);
            else WaveManager.Instance.GetPhaseCommands(wave);
            currentWave++;
            SaveManager.Save();
            //}
        }
        else if (newState == GameState.End)
        {
            State = GameState.End;
        }

        UIManager.Instance.OnGameStateChanged(newState, currentWave);
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
    End
}