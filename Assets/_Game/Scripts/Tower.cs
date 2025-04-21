using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public string TowerName;
    public int TowerPrice;
    public int GoldGenerated;
    public string ButtonTag;
    public TowerType Type;
    public int SellPrice;
    public int UpgradePrice;
    public GameObject TowerPanel;
    [HideInInspector] public Tile tile;

    public void TowerDestroyHandle()
    {
        Economics.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        Destroy(gameObject);

    }
}

public enum TowerType
{
    MainTower,
    ArcherTower,
    WizardTower,
    ArtilleryTower,
    GoldMine
}
