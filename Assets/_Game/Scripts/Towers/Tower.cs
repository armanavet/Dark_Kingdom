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
    protected float currentHP;
    public int MaxLevel = 3;
    public int CurrentLevel = 0;
    public int PurchasePrice;
    public string TowerName;
    public TowerType Type;
    public GameObject TowerPanel;

    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int GoldGenerated = 0;

    public TowerData saveData;

    public void Sell()
    {
        EconomyManager.Instance.ChangeGoldAmount(SellPrice);
        PrepareForDestruction();
        Destroy(gameObject);
    }
    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            PrepareForDestruction();
            Destroy(gameObject);
        }
    }

    void PrepareForDestruction()
    {
        TowerManager.Instance.Towers.Remove(this);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        tile.UnclaimNeighbors();
    }

    public TowerData OnSave()
    {
        return new TowerData(Type, tile.Index, CurrentLevel, currentHP);
    }

    public void OnLoad(TowerData data)
    {
        CurrentLevel = data.Level;
        currentHP = data.CurrentHP;
    }

    public abstract void Upgrade();
}
