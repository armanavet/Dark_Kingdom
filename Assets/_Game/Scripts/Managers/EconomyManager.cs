using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomyManager : MonoBehaviour, ISaveable
{
    [SerializeField] int currentGold;
    [SerializeField] List<Tower> EconomicBuildings = new List<Tower>();
    float timer = 0;
    public int CurrentGold { get => currentGold; }

    #region Singleton 
    private static EconomyManager _instance;
    public static EconomyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EconomyManager>();
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
        timer += Time.deltaTime;
        if(timer >= 1)
        {
            GenerateGold();
            timer = 0;
        }
    }
    
    public void ChangeGoldAmount(int amount)
    {
        currentGold += amount;
    }

    public void OnEconomicStructureChange(Tower structure)
    {
        if (structure == null) return;
        if (structure.Type != TowerType.GoldMine && structure.Type != TowerType.MainTower) return;

        if (EconomicBuildings.Contains(structure))
        {
            EconomicBuildings.Remove(structure);
            return;
        }
        EconomicBuildings.Add(structure);
    }

    void GenerateGold()
    {
        foreach (var building in EconomicBuildings)
        {
            if (building == null) continue;
            ChangeGoldAmount(building.GoldGenerated);
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
        saveData.CurrentGold = currentGold;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentGold = saveData.CurrentGold;
    }
}




