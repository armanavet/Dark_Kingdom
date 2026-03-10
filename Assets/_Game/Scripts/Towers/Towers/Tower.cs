using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    [Header("Tower Parameters")]
    [SerializeField] protected TowerType towerType;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected LayerMask illusionMask;
    [SerializeField] protected LayerMask portalMask;
    [SerializeField] protected List<int> UpgradePrices;
    [SerializeField] protected List<int> SellPrices;
    [SerializeField] protected List<float> HP;
    [SerializeField] protected List<int> Damage;
    [SerializeField] protected Debuff[] Debuffs;
    [SerializeField] protected GameObject[] Projectiles;
    [SerializeField] protected GameObject[] Models;
    [SerializeField] protected GameObject[] effects;
    [SerializeField] protected SoundData TowerSoundData;
    [SerializeField] protected SoundData SD_TowerUpgrade;
    [SerializeField] protected SoundData SD_TowerVfx;
    [SerializeField] protected HitPointPopup unitHitPointPopup;
    [SerializeField] protected List<float> canvasHeightByLevel;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected GameObject towerCanvas;
    [SerializeField] protected GameObject healthBarCanvas;

    protected float maxHP;
    protected float currentHP;
    protected List<Debuff> currentDebuffs = new List<Debuff>();
    protected GameObject projectile, model, effect;

    [HideInInspector] public Tile tile;
    [HideInInspector] public int SellPrice;
    [HideInInspector] public int UpgradePrice;
    [HideInInspector] public int CrystelGenerated = 0;
    [HideInInspector] public int LevelMax = 1;
    [HideInInspector] public int CurrentLevel = 0;
    [HideInInspector] public int PurchasePrice;
    [HideInInspector] public TowerType Type => towerType;
    [HideInInspector] public TowerData saveData;

    [HideInInspector] public GameObject TowerPanel => towerCanvas;
    [HideInInspector] public GameObject HealthBar => healthBarCanvas;
    public event System.Action OnDestroyed;

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
    void Destroy()
    {
        if (Type == TowerType.MainTower)
        {
            //StateManager.Instance.ChangeGameStateTo();
        }
        TowerManager.Instance.Towers.Remove(this);
        EconomyManager.Instance.OnEconomicStructureChange(this);
        tile.isEmpty = true;
        OnDestroyed?.Invoke();
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
    public virtual IEnumerator PlayUpdateSfx()
    {
        //AudioManager.Instance.Play(TowerSoundData, transform, ClipType.OnUpgradeVisualEffect_Tower, towerType);
        //effect = Instantiate(effects[2], new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        //effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Destroy(effect, 2f);
        yield break;
    }
    public virtual IEnumerator PlayPlaceSFX()
    {
        //AudioManager.Instance.Play(TowerSoundData, transform, ClipType.OnPlace_Tower, towerType);
        //AudioManager.Instance.Play(TowerSoundData, transform, ClipType.OnPlaceVisualEffect_Tower, towerType);
        //effect = Instantiate(effects[1], new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.Euler(-90f, transform.rotation.y, transform.rotation.z));
        //Destroy(effect, 2f);
        //effect = Instantiate(effects[0], new Vector3(transform.position.x, 0.8f, transform.position.z), Quaternion.identity);
        //Destroy(effect, 2f);
        yield break;
    }
}