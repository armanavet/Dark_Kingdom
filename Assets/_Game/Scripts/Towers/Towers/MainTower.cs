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
        model = Models[CurrentLevel];

        currentHP = currentHP == 0 ? maxHP : currentHP;
    }

    public override void Upgrade()
    {
        if (CurrentLevel < UpgradePrices.Count)
        {
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);

            CurrentLevel++; 

            damage = Damage[CurrentLevel];
            GoldGenerated = GoldGenerationList[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }
    }
}
