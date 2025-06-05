using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    float damage;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        GoldGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
    }

    public override void Upgrade()
    {
        if (CurrentLevel < UpgradePrices.Count)
        {
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            CurrentLevel++; 
            damage = Damage[CurrentLevel];
            GoldGenerated = GoldGenerationList[CurrentLevel];
            UpgradePrice = UpgradePrices[CurrentLevel];
            float hpPercent = currentHP / maxHP;
            currentHP = maxHP * hpPercent;
            maxHP = HP[CurrentLevel];
        }
    }
}
