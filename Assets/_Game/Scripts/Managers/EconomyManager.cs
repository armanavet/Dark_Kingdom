using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomyManager : MonoBehaviour, ISaveable
{
    [SerializeField] int currentCrystals;
    [SerializeField] List<Tower> EconomicBuildings = new List<Tower>();
    float timer = 0;
    public int CurrentCrystals { get => currentCrystals; }
    private bool canGenerate;

    #region Singleton 
    private static EconomyManager _instance;
    public static EconomyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EconomyManager>();
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

    void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;

        timer += Time.deltaTime * StateManager.Instance.timeMultiplier;

        CheckStrategy();
        if (timer >= 1 && canGenerate)
        {
            GenerateCrystals();
            timer = 0;
        }
    }

    public void ChangeCrystals(int amount)
    {
        currentCrystals += amount;
    }

    public void OnEconomicStructureChange(Tower structure)
    {
        if (structure == null) return;
        if (structure.Type != TowerType.CrystalMine && structure.Type != TowerType.MainTower) return;

        if (EconomicBuildings.Contains(structure))
        {
            EconomicBuildings.Remove(structure);
            return;
        }
        EconomicBuildings.Add(structure);
    }

    void GenerateCrystals()
    {
        foreach (var building in EconomicBuildings)
        {
            if (building == null) continue;
            ChangeCrystals(building.CrystalsGenerated);
        }
    }

    private void CheckStrategy()
    {
        if (StrategyManager.Instance.CurrentStrategy == StrategyType.Economy)
        {
            canGenerate = true;
        }
        else
        {
            canGenerate = false;
        }
    }

    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    public string GetUniqueSaveID()
    {
        return nameof(EconomyManager);
    }

    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.CurrentCrystals = currentCrystals;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentCrystals = saveData.CurrentCrystals;
    }
}




