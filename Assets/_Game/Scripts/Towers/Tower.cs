using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected List<int> SellPrices;
    [SerializeField] protected List<int> UpgradePrices;

    public int maxLevel = 2;
    public int levelOFTower = 0;
    public int TowerPrice;
    public string TowerName;
    public TowerType Type;
    public GameObject TowerPanel;

    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int GoldGenerated = 0;


    private void Start()
    {
        SellPrice = SellPrices[levelOFTower];
        UpgradePrice = UpgradePrices[levelOFTower];
    }
    public void Destroy()
    {
        EconomyManager.Instance.OnEconomyManagertructureChange(this);
        tile.isEmpty = true;
        Destroy(gameObject);
    }
    public abstract void Upgrade();
}

public enum TowerType
{
    MainTower,
    ArcherTower,
    WizardTower,
    ArtilleryTower,
    GoldMine
}
