using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Statemanager : MonoBehaviour
{
    static int Wave;
    static float Timer;
    [SerializeField] TextMeshProUGUI WaveText;
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] TextMeshProUGUI EnemiesText;
    [SerializeField] float[] TimeUntilNextWave;
    public static bool Active = false;
    public static bool Passive = true;
    [SerializeField] GameObject ActiveText, PassiveText;
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
    void Start()
    {
        Wave = 1;
        Timer = TimeUntilNextWave[0];
        ActiveText.SetActive(false);
        PassiveText.SetActive(true);
    }
    void Update()
    {
        WaveText.text = Wave.ToString();
        TimerText.text = Mathf.Round(Timer).ToString();
        Timer -=Time.deltaTime;
        EnumGameState();
    }
    public static void EnumGameState()
    {
        if (Timer <= 0 && Passive)
        {
            Active = true;
            Passive = false;
            _instance.ActiveText.SetActive(true);
            _instance.PassiveText.SetActive(false);
            _instance.EnemiesText.text = "Enemies Attack";
        }
        else if (Input.GetKeyUp(KeyCode.N) && Active)
        {
            Wave++;
            Active = false;
            Passive = true;
            _instance.PassiveText.SetActive(true);
            _instance.ActiveText.SetActive(false);
            Timer = _instance.TimeUntilNextWave[Wave];
        }
    }
}


