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
        if (Economics.Instance.CurrentGold < tower.UpgradePrice)
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
        Economics.Instance.ChangeGoldAmount(tower.SellPrice);
        tower.TowerDestroyHandle();
    }
    public void ButtonTowerUpgrade() 
    {
        Economics.Instance.ChangeGoldAmount(-tower.UpgradePrice);
    }
}
