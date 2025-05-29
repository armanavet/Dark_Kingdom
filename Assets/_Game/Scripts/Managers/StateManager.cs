using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WaveText, TimerText;
    [SerializeField] GameObject ActiveStatePanel, PassiveStatePanel;
    [SerializeField] float[] TimeUntilNextWave;
    [HideInInspector] public GameState State;
    float Timer;
    int timeMultiplier = 1;
    int CurrentWave = 0;

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
    }
    #endregion

    void Start()
    {
        ChangeGameStateTo(GameState.Passive);
    }

    void Update()
    {
        if (State == GameState.Passive)
        {
            Timer -= Time.deltaTime * timeMultiplier;
            TimerText.text = Mathf.Round(Timer).ToString();
            if (Timer <= 0) ChangeGameStateTo(GameState.Active);
        }
        else if (State == GameState.Active)
        {
            WaveText.text = "Wave: " + CurrentWave.ToString();
        }
        
    }

    public void ChangeGameStateTo(GameState newState)
    {
        if (newState == GameState.Active)
        {
            State = GameState.Active;
            timeMultiplier = 1;
            ActiveStatePanel.SetActive(true);
            PassiveStatePanel.SetActive(false);
            WaveManager.Instance.SpawnWave(CurrentWave - 1);
        }
        else if (newState == GameState.Passive)
        {
            State = GameState.Passive;
            CurrentWave++;
            ActiveStatePanel.SetActive(false);
            PassiveStatePanel.SetActive(true);
            Timer = (CurrentWave <= TimeUntilNextWave.Length) ? TimeUntilNextWave[CurrentWave - 1] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
            WaveManager.Instance.DrawEnemyPath();
        }
    }

    public void ButtonToMakeFaster(int multiplier)
    {
        timeMultiplier = (timeMultiplier == multiplier) ? (timeMultiplier = 1) : (timeMultiplier = multiplier);
    }
}

public enum GameState
{
    Active,
    Passive
}