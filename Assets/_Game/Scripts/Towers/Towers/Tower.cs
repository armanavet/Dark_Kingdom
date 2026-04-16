using AudioSystem;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour, ITargetable
{
    [Header("Tower Parameters")]
    [SerializeField] protected TowerType towerType;
    [SerializeField] protected LayerMask capturedMask;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected LayerMask illusionMask;
    [SerializeField] protected LayerMask portalMask;
    [SerializeField] protected LayerMask hittabelMask;
    [SerializeField] protected List<int> UpgradePrices;
    [SerializeField] protected List<int> SellPrices;
    [SerializeField] protected List<float> HP;
    [SerializeField] protected List<int> Damage;
    [SerializeField] protected Debuff[] Debuffs;
    [SerializeField] protected GameObject[] Projectiles;
    [SerializeField] protected GameObject[] Models;
    [SerializeField] protected GameObject[] effects;
    [SerializeField] protected SoundData towerSoundData;
    [SerializeField] protected HitPointPopup unitHitPointPopup;
    [SerializeField] protected List<float> canvasHeightByLevel;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected GameObject towerCanvas;
    [SerializeField] protected GameObject healthBarCanvas;
    [SerializeField] protected float cooldown;

    protected float maxHP;
    protected float currentHP;
    protected bool isCaptured;
    protected List<Debuff> currentDebuffs = new List<Debuff>();
    protected GameObject projectile, model, effect;

    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int CrystalGenerated = 0;
    [HideInInspector] public int LevelMax = 1;
    [HideInInspector] public int CurrentLevel = 0;
    [HideInInspector] public int PurchasePrice;
    [HideInInspector] public TowerType Type => towerType;
    [HideInInspector] public TowerData saveData;

    [HideInInspector] public GameObject TowerPanel => towerCanvas;
    [HideInInspector] public GameObject HealthBar => healthBarCanvas;
    [HideInInspector] public SoundData SoundData => towerSoundData;
    public event Action<Tower> OnDestroyed;
    public Faction faction = Faction.Player;
    public Transform GetTransform() => transform;
    public Faction GetFaction() => faction;
    public abstract int GetTargetPriority();
    public float GetHealthPrecent() { return currentHP / maxHP; }
    public void Sell(int price)
    {
        EconomyManager.Instance.ChangeCrystelAmount(price);
        Destroy();
    }
    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        healthBar.SetHealth(currentHP);
        if (Type == TowerType.MainTower) UIManager.Instance.MainTowerHB.SetHealth(currentHP);
        if (currentHP <= 0)
        {
            Destroy();
        }
    }
    int GetLayerFromMask(LayerMask mask)
    {
        int value = mask.value;

        if (value == 0 || (value & (value - 1)) != 0)
        {
            return 0;
        }
        return Mathf.RoundToInt(MathF.Log(value, 2));
    }
    //public void SetPriority(LayerMask newMask, LayerMask targetMask)
    public void SetPriority(Faction faction)
    {
        //gameObject.layer = GetLayerFromMask(newMask);
        //enemyMask = targetMask;
        //Debug.Log($"Tower {gameObject} mask was changed! Tower layer: {LayerMask.LayerToName(GetLayerFromMask(gameObject.layer))}. Enemy mask: {LayerMask.LayerToName(GetLayerFromMask(enemyMask))}");
        this.faction = faction;
        Debug.Log($"Tower faction is {this.faction}");
        isCaptured = !isCaptured;
    }
    protected void Destroy()
    {
        if (Type == TowerType.MainTower)
        {
            //StateManager.Instance.ChangeGameStateTo();
        }
        TowerManager.Instance.Towers.Remove(this);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        towerSoundData = new SoundData();
        OnDestroyed?.Invoke(this);
        Debug.Log($"Tower {gameObject} was destroyed!");
        Destroy(gameObject);
    }
    public void UpdateCanvasHeight(int levelIndex)
    {
        if (canvasHeightByLevel == null || canvasHeightByLevel.Count == 0)
        {
            Debug.LogWarning("Canvas height configuration is missing.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= canvasHeightByLevel.Count)
        {
            Debug.LogWarning("Level index is out of range.");
            return;
        }

        float targetHeight = canvasHeightByLevel[levelIndex];

        if (Type != TowerType.MainTower) ApplyHeight(healthBarCanvas.transform);
        ApplyHeight(towerCanvas.transform);

        void ApplyHeight(Transform canvasTransform)
        {
            if (canvasTransform == null) return;

            canvasTransform.position = SetY(canvasTransform.position, targetHeight);
        }
    }

    private Vector3 SetY(Vector3 position, float newY)
    {
        position.y = newY;
        return position;
    }
    protected void SetSoundData()
    {
        towerSoundData = AudioManager.Instance.SetData(Type, SoundDataType.Gameplay);
    }
    protected void OnUpgrade()
    {
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerUpgradeVFX, SoundData, transform);
        Debug.Log(SoundData.volume);
        int index = (Type == TowerType.MainTower) ? 0 : 2;
        effect = Instantiate
            (effects[index]
            , new Vector3(transform.position.x, 0.5f, transform.position.z)
            , Quaternion.identity);

        effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        float duration = effect.GetComponent<ParticleSystem>().main.startLifetime.constantMax;
        Destroy(effect, duration);
    }
    protected void OnPlace()
    {
        float duration = 0;
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerPlace, SoundData, transform);
        SoundData.volume = 1f;
        AudioManager.Instance.Play(GamePlaySFX_Type.TowerPuffVFX, SoundData, transform);
        effect = Instantiate
            (effects[1],
            new Vector3(transform.position.x, 0.15f, transform.position.z),
            Quaternion.identity);

        duration = effect.GetComponent<ParticleSystem>().main.startLifetime.constantMax;
        Destroy(effect, duration);

        effect = Instantiate
            (effects[0],
            new Vector3(transform.position.x, 0.8f, transform.position.z),
            Quaternion.identity);

        duration = effect.GetComponent<ParticleSystem>().main.startLifetime.constantMax;
        Destroy(effect, duration);
    }
    public TowerData OnSave()
    {
        return new TowerData(Type, tile.Index, CurrentLevel, currentHP);
    }

    public void OnLoad(TowerData data)
    {
        CurrentLevel = data.Level;
        currentHP = data.CurrentHP;
        if (Type == TowerType.MainTower) UIManager.Instance.MainTowerHB.SetHealth(currentHP);
    }

    public abstract void Upgrade();

}