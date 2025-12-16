using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] SoundData TowerPanelSoundData;
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
        //AudioManager.Instance.Play(TowerPanelSoundData,ClipType.OnSellButtonClick_UI);
        tower.Sell(tower.SellPrice);
    }
    public void ButtonTowerUpgrade()
    {
        //AudioManager.Instance.Play(TowerPanelSoundData,ClipType.OnUpgradeButtonClick_UI);
        tower.Upgrade();
    }
}
