using System;
using System.Collections.Generic;
using UnityEngine;

public class StrategyManager : MonoBehaviour, ISaveable
{
    #region Singleton 
    private static StrategyManager _instance;
    public static StrategyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<StrategyManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private StrategyDatabaseSO strategyDB;
    [SerializeField] private StrategyType currentStrategy;
    [SerializeField] private float timer;
    private bool canSwitch = true;

    public StrategyType CurrentStrategy => currentStrategy;
    public static event Action<StrategyType> OnStrategyChanged;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            canSwitch = true;
        }
    }

    public void ChangeStrategy(int strategyIndex)
    {
        if (!canSwitch)
        {
            Debug.Log("Can't switch strategy yet.");
            return;
        }

        StrategyType newStrategy = (StrategyType)strategyIndex;
        if (newStrategy == currentStrategy) return;

        currentStrategy = newStrategy;
        Strategy s = strategyDB.GetByType(newStrategy);
        timer = s.Cooldown;
        canSwitch = false;

        OnStrategyChanged?.Invoke(newStrategy);
    }

    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    public string GetUniqueSaveID()
    {
        return nameof(StrategyManager);
    }

    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.CurrentStrategy = currentStrategy;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentStrategy = saveData.CurrentStrategy;
        OnStrategyChanged?.Invoke(currentStrategy);
    }
}
