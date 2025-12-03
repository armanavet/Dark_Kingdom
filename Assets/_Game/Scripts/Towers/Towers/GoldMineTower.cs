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
        GoldGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
        PlayTowerSFX(SD_TowerAction, PlaceSfx);
        PlayTowerSFX(SD_TowerVfx, VfxSfx);
        effect = Instantiate(effects[1], new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.Euler(-90f, transform.rotation.y, transform.rotation.z));
        Destroy(effect, 2f);
        effect = Instantiate(effects[0], new Vector3(transform.position.x, 0.8f, transform.position.z), Quaternion.identity);
        Destroy(effect, 2f);
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
           StartCoroutine(PlayUpdateSfxs());
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            GoldGenerated = GoldGenerationList[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }
        StopCoroutine(PlayUpdateSfxs());
    }
}
