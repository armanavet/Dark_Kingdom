using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTower : Tower
{
    public override void Upgrade()
    {
        if (levelOFTower < SellPrices.Count - 1 && levelOFTower < UpgradePrices.Count)
        {
            SellPrice = SellPrices[levelOFTower + 1];
            UpgradePrice = UpgradePrices[levelOFTower];
            Economics.Instance.ChangeGoldAmount(-UpgradePrice);
            levelOFTower++;
        }
    }
}
