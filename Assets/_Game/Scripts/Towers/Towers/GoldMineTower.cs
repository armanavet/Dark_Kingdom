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
        model = Models[CurrentLevel];

        currentHP = currentHP == 0 ? maxHP : currentHP;
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);

            CurrentLevel++;

            GoldGenerated = GoldGenerationList[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }
    }
}
