using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomyManager : MonoBehaviour, ISaveable
{
    [SerializeField] int currentCrystal;
    [SerializeField] List<Tower> EconomicBuildings = new List<Tower>();
    float timer = 0;
    public int CurrentGold { get => currentCrystal; }

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
        timer += Time.deltaTime * StateManager.Instance.timeMultiplier;
        if (timer >= 1)
        {
            CurrentCrystel();
            timer = 0;
        }
    }

    public void ChangeCrystelAmount(int amount)
    {
        currentCrystal += amount;
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

    void CurrentCrystel()
    {
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Economy) return;

        foreach (var building in EconomicBuildings)
        {
            if (building == null) continue;
            ChangeCrystelAmount(building.CrystelGenerated);
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
        saveData.CurrentCrystals = currentCrystal;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentCrystal = saveData.CurrentCrystals;
    }
}




