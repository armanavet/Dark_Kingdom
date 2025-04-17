using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Economics : MonoBehaviour
{
    [SerializeField] int currentGold;
    public int CurrentGold { get => currentGold; }
    List<Tower> EconomicBuilding = new List<Tower>();
    float timer = 0;
    #region Singleton 
    private static Economics _instance;
    public static Economics Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Economics>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Update()
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
        if (structure.Type != TowerType.GoldMine && structure.Type != TowerType.MainTower) return;
        if (EconomicBuilding.Contains(structure))
        {
            EconomicBuilding.Remove(structure);
            return;
        }
        EconomicBuilding.Add(structure);
    }

    void GenerateGold()
    {
        foreach (var building in EconomicBuilding)
        {
            ChangeGoldAmount(building.GoldGenerated);
        }

       
    }
}




