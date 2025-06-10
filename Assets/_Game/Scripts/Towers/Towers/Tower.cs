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
    [SerializeField] protected Debuff[] debuffs;
    [SerializeField] protected GameObject[] Projectiles;
    protected float maxHP;
    protected float currentHP;
    protected List<Debuff> currentDebuffs = new List<Debuff>();
    protected GameObject projectile;
    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int GoldGenerated = 0;
    [HideInInspector] public int MaxLevel = 3;
    [HideInInspector] public int CurrentLevel = 0;
    [HideInInspector] public int PurchasePrice;
    [HideInInspector] public TowerType Type;
    [HideInInspector] public TowerData saveData;
    public GameObject TowerPanel;

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
