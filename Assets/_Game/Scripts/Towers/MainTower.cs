using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        GoldGenerated = GoldGenerationList[levelOFTower];
        maxHP = HP[levelOFTower];
        currentHP = maxHP;
    }
    public override void Upgrade()
    {
        if (levelOFTower < UpgradePrices.Count)
        {
            float hpPercent=currentHP/maxHP;
            UpgradePrice = UpgradePrices[levelOFTower];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            levelOFTower++; 
            maxHP = HP[levelOFTower];
            currentHP = maxHP*hpPercent;
            GoldGenerated = GoldGenerationList[levelOFTower];
        }
    }
}
