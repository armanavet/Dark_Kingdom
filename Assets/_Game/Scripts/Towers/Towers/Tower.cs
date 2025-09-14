using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    [Header("Tower parameters")]
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected LayerMask illusionMask;
    [SerializeField] protected List<int> UpgradePrices;
    [SerializeField] protected List<int> SellPrices;
    [SerializeField] protected List<float> HP;
    [SerializeField] protected List<int> Damage;
    [SerializeField] protected Debuff[] Debuffs;
    [SerializeField] protected GameObject[] Projectiles;
    [SerializeField] protected GameObject[] Models; 
    
    protected AudioSource towerAudioSource;
    protected float maxHP;
    protected float currentHP;
    protected List<Debuff> currentDebuffs = new List<Debuff>();
    protected GameObject projectile;
    protected GameObject model;
    
    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int GoldGenerated = 0;
    [HideInInspector] public int LevelMax = 1;
    [HideInInspector] public int CurrentLevel = 0;
    [HideInInspector] public int PurchasePrice;
    [HideInInspector] public TowerType Type;
    [HideInInspector] public TowerData saveData;
    
    public GameObject TowerPanel;
    public event System.Action OnDestroyed;

    public void Sell()
    {
        EconomyManager.Instance.ChangeGoldAmount(SellPrice);
        Destroy();
    }
    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Destroy();
        }
    }
    public void TowerAudio(AudioClip audioClip, AudioSource audioSource)
    {
        AudioManager.instance.PlayTowerActionSound(audioClip, audioSource);
    }
    void Destroy()
    {
        TowerManager.Instance.Towers.Remove(this);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        OnDestroyed?.Invoke();
        Destroy(gameObject);
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
