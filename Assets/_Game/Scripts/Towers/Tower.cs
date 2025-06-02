using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected List<int> SellPrices;
    [SerializeField] protected List<int> UpgradePrices;
    [SerializeField] protected List<float> HP;
    [SerializeField] protected List<int> Damage;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
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

    public void Sell()
    {
        Economics.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        GameBoard.Instance.BuildPathToDestination();
        SellPrice = SellPrices[levelOFTower];
        Economics.Instance.ChangeGoldAmount(SellPrice);
        Destroy(gameObject);
    }
    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
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
