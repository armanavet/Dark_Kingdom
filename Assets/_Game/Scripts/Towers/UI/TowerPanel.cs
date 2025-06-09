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
        if (EconomyManager.Instance.CurrentGold < tower.UpgradePrice || tower.levelOFTower > tower.maxLevel)
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
        EconomyManager.Instance.ChangeGoldAmount(tower.SellPrice);
        tower.Sell();
    }
    public void ButtonTowerUpgrade()
    {
        tower.Upgrade();
    }
}
