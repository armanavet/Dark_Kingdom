using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Statemanager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WaveText, TimerText, EnemiesText;
    [SerializeField] GameObject ActiveStateText, PassiveStateText;
    [SerializeField] float[] TimeUntilNextWave;
    int CurentWave = 1;
    float Timer;
    [HideInInspector] public States State = States.Passive;
    int timeMultiplier = 1;

    #region Singleton
    private static Statemanager _instance;
    public static Statemanager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Statemanager>();
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
        Timer = TimeUntilNextWave[0];
        ActiveStateText.SetActive(false);
        PassiveStateText.SetActive(true);
    }

    void Update()
    {
        WaveText.text = "Wave: " + CurentWave.ToString();
        TimerText.text = Mathf.Round(Timer).ToString();
        Timer -= Time.deltaTime * timeMultiplier;
        EvaluateGameState();
    }

    void EvaluateGameState()
    {
        if (Timer <= 0 && State == States.Passive)
        {
            State = States.Active;
            timeMultiplier = 1;
            ActiveStateText.SetActive(true);
            PassiveStateText.SetActive(false);
            EnemiesText.text = "Enemies Are Attacking";
        }
        else if (Input.GetKeyUp(KeyCode.N) && State == States.Active)
        {
            CurentWave++;
            State = States.Passive;
            ActiveStateText.SetActive(false);
            PassiveStateText.SetActive(true);
            Timer = (CurentWave <= TimeUntilNextWave.Length) ? TimeUntilNextWave[CurentWave - 1] : TimeUntilNextWave[TimeUntilNextWave.Length - 1];
        }
    }

    public void ButtonToMakeFaster(int multiplier)
    {
        timeMultiplier = (timeMultiplier == multiplier) ? (timeMultiplier = 1) : (timeMultiplier = multiplier);
    }
}

public enum States
{
    Active,
    Passive
}