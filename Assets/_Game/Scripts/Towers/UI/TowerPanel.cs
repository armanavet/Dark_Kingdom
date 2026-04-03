using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] Button UpgradeButton;
    [SerializeField] Button SellButton;
    Tower tower;

    private void Start()
    {
        tower = GetComponent<Tower>();
    }
    private void Update()
    {
        ChangeButtonVisibility();
    }

    void ChangeButtonVisibility()
    {
        if (EconomyManager.Instance.CurrentCrystal < tower.UpgradePrice || tower.CurrentLevel > tower.LevelMax || StrategyManager.Instance.CurrentStrategy != StrategyType.Construction)
        {
            UpgradeButton.interactable = false;
        }
        else
        {
            UpgradeButton.interactable = true;
        }

        if (tower.Type != TowerType.MainTower && StrategyManager.Instance.CurrentStrategy != StrategyType.Construction)
        {
            SellButton.interactable = false;
        }
        else if (tower.Type != TowerType.MainTower)
        {
            SellButton.interactable = true;
        }
    }
    public void ButtonTowerSell()
    {
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Construction) return;

        AudioManager.Instance.Play(UISFX_Type.TowerSellButton, tower.SoundData);
        tower.Sell(tower.SellPrice);
    }
    public void ButtonTowerUpgrade()
    {
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Construction) return;

        AudioManager.Instance.Play(UISFX_Type.TowerUpgradeButton, tower.SoundData);
        tower.Upgrade();
    }
}
