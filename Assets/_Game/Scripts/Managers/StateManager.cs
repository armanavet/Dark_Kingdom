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
    int timeMultiplier = 1;
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
        
    }
    public void ChangeGameStateTo(GameState newState)
    {
        if (newState == GameState.Active)
        {
            State = GameState.Active;
            timeMultiplier = 1;
            WaveManager.Instance.SpawnWave(currentWave - 1);
        }
        else if (newState == GameState.Passive)
        {
            State = GameState.Passive;
            currentWave++;
            Timer = (currentWave <= TimeUntilNextWave.Length) ? TimeUntilNextWave[currentWave - 1] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
            WaveManager.Instance.DrawEnemyPath();
            SaveManager.Save();
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
    Paused
}