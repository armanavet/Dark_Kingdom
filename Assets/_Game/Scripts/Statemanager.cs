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
    public static bool ActiveState = false;
    public static bool PassiveState = true;
    int CurentWave = 1;
    float Timer;

    bool toMakeX2Faster = false;

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
        WaveText.text = CurentWave.ToString();
        TimerText.text = Mathf.Round(Timer).ToString();
        
        if (toMakeX2Faster)
        {
            Timer -= Time.deltaTime *2;

        }
        else
        {
            Timer -= Time.deltaTime;
        }
        EvaluateGameState();
    }

    void EvaluateGameState()
    {
        if (Timer <= 0 && PassiveState)
        {
            ActiveState = true;
            PassiveState = false;
            ActiveStateText.SetActive(true);
            PassiveStateText.SetActive(false);
            EnemiesText.text = "Enemies Are Attacking";
        }
        else if (Input.GetKeyUp(KeyCode.N) && ActiveState)
        {
            CurentWave++;
            ActiveState = false;
            PassiveState = true;
            ActiveStateText.SetActive(false);
            PassiveStateText.SetActive(true);
            Timer = TimeUntilNextWave[CurentWave - 1];
        }
    }

    public void ButtonToMakeX2Faster()
    {
        toMakeX2Faster = !toMakeX2Faster;
    }
}