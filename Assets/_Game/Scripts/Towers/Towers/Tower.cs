using AudioSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour, ITargetable
{
    [Header("References")]
    [SerializeField] protected TowerDataSO data;
    [SerializeField] protected GameObject[] models;
    [SerializeField] protected Canvas worldCanvas;
    [SerializeField] protected GameObject[] sleepFX;
    [HideInInspector] public Tile Tile;
    public TowerPanel TowerPanel;
    public HealthBar HealthBar;
    public LayerMask TargetMask;


    //TODO remove ALL serialization below
    [Header("General data")]
    [SerializeField] protected SoundData soundData;
    [SerializeField] protected float currentHP;
    protected int currentLevel = 1;
    [SerializeField] protected Dictionary<DebuffType, Debuff> currentDebuffs = new();
    protected float range { get => data.Range; }
    protected float cooldown { get => data.Cooldown; }

    [Header("Level-based data")]
    protected GameObject projectile { get => data.Levels[currentLevel - 1].Projectile; }
    protected float maxHP { get => data.Levels[currentLevel - 1].HP; }
    protected float damage { get => data.Levels[currentLevel - 1].Damage; }
    protected int sellPrice { get => data.Levels[currentLevel - 1].SellPrice; }

    [Header("Public data")]
    public TowerType Type { get => data.Type; }
    public Faction Faction { get => data.Faction; }
    public Transform Transform { get => transform; }
    public int TargetPriority { get => data.TargetPriority; }
    public int PurchasePrice { get => data.PurchasePrice; }
    public int UpgradePrice { get => data.Levels[currentLevel - 1].UpgradePrice; }
    public int CrystalsGenerated { get => data.Levels[currentLevel - 1].CrystalsGenerated; }
    public float HealthPercent { get => currentHP / maxHP; }
    public bool IsMaxLevel { get => currentLevel >= data.Levels.Length; }
    public bool IsDestroyed { get; protected set; }


    public event Action<Tower> OnDestroyed;

    protected virtual void Start()
    {
        soundData = AudioManager.Instance.SetData(Type, SoundDataType.Gameplay);
        worldCanvas.worldCamera = Camera.main;
        UpdateData();
        OnPlace();
    }

    public void Sell()
    {
        EconomyManager.Instance.ChangeCrystals(sellPrice);
        Destroy();
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        HealthBar.SetHealth(currentHP);
        if (Type == TowerType.MainTower) UIManager.Instance.MainTowerHB.SetHealth(currentHP);
        if (currentHP <= 0)
        {
            Destroy();
        }
    }

    protected void Destroy()
    {
        if (Type == TowerType.MainTower)
        {
            //StateManager.Instance.ChangeGameStateTo();
        }
        IsDestroyed = true;
        TowerManager.Instance.Towers.Remove(this);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        Tile.isEmpty = true;
        soundData = new SoundData();
        OnDestroyed?.Invoke(this);
        Debug.Log($"Tower {gameObject} was destroyed!");
        Destroy(gameObject);
    }

    //public void SetFaction(Faction faction)
    //{
    //    Faction = faction;
    //    Debug.Log($"Tower faction is {Faction}");
    //    IsCaptured = !IsCaptured;
    //}

    public void Upgrade()
    {
        if (IsMaxLevel) return;

        EconomyManager.Instance.ChangeCrystals(-UpgradePrice);
        models[currentLevel - 1].SetActive(false);
        float hpPercent = HealthPercent;
        currentLevel++;
        currentHP = maxHP * hpPercent;
        UpdateData();
        
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerUpgradeVFX, soundData, transform);
        Debug.Log(soundData.volume);
        var effect = Instantiate(data.UpgradeEffect);
        effect.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    protected virtual void UpdateData()
    {
        HealthBar.SetMaxHealth(maxHP);
        worldCanvas.transform.localPosition = new Vector3(0, data.Levels[currentLevel - 1].CanvasHeight, 0);
        models[currentLevel - 1].SetActive(true);

        Debuff debuff = data.Levels[currentLevel - 1].Debuff;
        if (debuff != null)
            currentDebuffs[debuff.Type] = debuff;
    }

    protected void OnPlace()
    {
        currentHP = maxHP;
        TowerPanel.Tower = this;
        TowerPanel.gameObject.SetActive(false);
        EconomyManager.Instance.ChangeCrystals(-data.PurchasePrice);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerPlace, soundData, transform);
        soundData.volume = 1f;
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerPuffVFX, soundData, transform);

        Instantiate(data.BuildEffectLow, new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.identity);
        Instantiate(data.BuildEffectHigh, new Vector3(transform.position.x, 0.8f, transform.position.z), Quaternion.identity);
    }

    protected abstract void CheckStrategy();

    public TowerData OnSave()
    {
        return new TowerData(Type, Tile.Index, currentLevel, currentHP);
    }

    public void OnLoad(TowerData data)
    {
        currentLevel = data.Level;
        UpdateData();
        currentHP = data.CurrentHP;
        if (Type == TowerType.MainTower)
            UIManager.Instance.MainTowerHB.SetHealth(currentHP);
    }
}