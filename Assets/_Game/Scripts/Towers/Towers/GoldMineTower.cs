using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMineTower : Tower
{
    [Header("Wizard Tower Parameters")]
    [SerializeField] List<int> CrystalGenerationList;
    public override int GetTargetPriority() => 0;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        CrystalGenerated = CrystalGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
        healthBar.SetMaxHealth(currentHP);
        UpdateCanvasHeight(CurrentLevel);
        towerCanvas.SetActive(false);
        OnPlace();
        SetSoundData();
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            OnUpgrade();
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeCrystelAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            CrystalGenerated = CrystalGenerationList[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);
            UpdateCanvasHeight(CurrentLevel);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
            healthBar.SetMaxHealth(currentHP);
        }
    }
}
