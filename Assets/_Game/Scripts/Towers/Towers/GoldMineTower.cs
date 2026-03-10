using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMineTower : Tower
{
    [Header("Wizard Tower Parameters")]
    [SerializeField] List<int> GoldGenerationList;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        CrystelGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP; 
        healthBar.SetMaxHealth(currentHP);
        UpdateCanvasHeight(CurrentLevel);
        StartCoroutine(PlayPlaceSFX());
        //TowerSoundData = AudioManager.Instance.SetData(TowerSoundData, SoundDataType.Tower, MixerType.Tower,towerType);
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
           //StartCoroutine(PlayUpdateSfx());
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeCrystelAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            CrystelGenerated = GoldGenerationList[CurrentLevel];
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
        //StopCoroutine(PlayUpdateSfx());
    }
}
