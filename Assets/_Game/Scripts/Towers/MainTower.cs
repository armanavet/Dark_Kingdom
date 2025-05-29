using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    private void Start()
    {
        EconomyManager.Instance.OnEconomyManagertructureChange(this);
        GoldGenerated = GoldGenerationList[levelOFTower];
    }
    public override void Upgrade()
    {
        if (levelOFTower < UpgradePrices.Count)
        {
            UpgradePrice = UpgradePrices[levelOFTower];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            levelOFTower++; 
            GoldGenerated = GoldGenerationList[levelOFTower];
        }
    }
}
