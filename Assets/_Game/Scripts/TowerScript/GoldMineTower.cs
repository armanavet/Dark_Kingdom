using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMineTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    private void Start()
    {
        Economics.Instance.OnEconomicStructureChange(this);
        GoldGenerated = GoldGenerationList[levelOFTower];
        maxHP = HP[levelOFTower];
        currentHP = maxHP;
    }
    public override void Upgrade()
    {
        if (levelOFTower < SellPrices.Count - 1 && levelOFTower < UpgradePrices.Count)
        {
            SellPrice = SellPrices[levelOFTower + 1];
            UpgradePrice = UpgradePrices[levelOFTower];
            Economics.Instance.ChangeGoldAmount(-UpgradePrice);
            GoldGenerated = GoldGenerationList[levelOFTower];
            levelOFTower++;
            
        }
    }
}
