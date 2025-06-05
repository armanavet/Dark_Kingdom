using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMineTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        GoldGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        maxHP = HP[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            CurrentLevel++;
            GoldGenerated = GoldGenerationList[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];
            UpgradePrice = UpgradePrices[CurrentLevel];
            float hpPercent = currentHP / maxHP;
            currentHP = maxHP * hpPercent;
            maxHP = HP[CurrentLevel];
        }
    }
}
