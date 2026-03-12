using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] Button UpgradeButton;
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
        if (EconomyManager.Instance.CurrentGold < tower.UpgradePrice || tower.CurrentLevel > tower.LevelMax)
        {
            UpgradeButton.interactable = false;
        }
        else
        {
            UpgradeButton.interactable = true;
        }
    }
    public void ButtonTowerSell()
    {
        AudioManager.Instance.Play(UISFX_Type.TowerSellButton, tower.SoundData);
        tower.Sell(tower.SellPrice);
    }
    public void ButtonTowerUpgrade()
    {
        AudioManager.Instance.Play(UISFX_Type.TowerUpgradeButton, tower.SoundData);
        tower.Upgrade();
    }
}
