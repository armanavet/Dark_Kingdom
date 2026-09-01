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
                _instance = FindFirstObjectByType<StrategyManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private StrategyUIManager uiManager;
    [SerializeField] private StrategyDatabaseSO strategyDB;
    [SerializeField] private StrategyType currentStrategy;
    private float timer;
    private float cooldown;
    private bool canSwitch = true;

    public StrategyType CurrentStrategy => currentStrategy;
    public static event Action<Strategy> OnStrategyChanged;

    public void Initialize()
    {
        uiManager.Initialize();
        ChangeStrategy(0);
        timer = cooldown = 0;
    }

    private void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;

        timer -= Time.deltaTime;
        uiManager.UpdateCooldown(timer, cooldown);
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
        timer = cooldown = s.Cooldown;
        canSwitch = false;

        OnStrategyChanged.Invoke(s);
        uiManager.OnStrategyChanged(s);
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
        ChangeStrategy((int)currentStrategy);
    }
}
