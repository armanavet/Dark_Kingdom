using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPanel : MonoBehaviour
{
    Tower tower;

    private void Start()
    {
        tower = GetComponent<Tower>();
    }
    public void ButtonTowerSell(){
       
        Economics.Instance.OnEconomicStructureChange(tower);
        Debug.Log(tower.SellPrice);
        Economics.Instance.ChangeGoldAmount(tower.SellPrice);
        Destroy(gameObject);

    }
    public void ButtonTowerUpgrade() {

        Debug.Log(tower.UpgradePrice);
        Economics.Instance.ChangeGoldAmount(-tower.UpgradePrice);
    }
}
