using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryTower : Tower
{
    [Header("Artillery Tower Parameters")]
    [SerializeField] Transform mortal;
    [SerializeField] Shell shell;
    [SerializeField, Range(1, 10f)]
    float TarggetPoint = 2f;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1;
    [SerializeField, Range(1, 200)]
    float shellDamage = 30;

    ITargetable target;
    float TarggetRange = 2f;
    float g = 9.81f;
    float launchSpeed;
    float launchProgress = 0f;
    int shotsPerSecond = 1;
    public override int GetTargetPriority() => 70;

    void Awake()
    {
        float x = TarggetRange + 0.250001f;
        float y = -mortal.position.y;
        launchSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
    }
    void Start()
    {
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        shellDamage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        if (Debuffs[CurrentLevel] != null)
            currentDebuffs.Add(Debuffs[CurrentLevel]);
        currentHP = currentHP == 0 ? maxHP : currentHP;
        healthBar.SetMaxHealth(currentHP);
        OnPlace();
        UpdateCanvasHeight(CurrentLevel);
        towerCanvas.SetActive(false);
        SetSoundData();
    }

    void Update()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Battle) return;

        if (launchProgress > 4)
        {
            if (AcquireTarget())
            {
                Launch(target);
            }
            launchProgress = 0;
        }
    }

    void Launch(ITargetable target)
    {
        if (target == null)
        {
            return;
        }
        Vector2 dir;
        Vector3 launchPoint = mortal.position;
        Vector3 TargetPoint = target.GetTransform().position;
        dir.x = TargetPoint.x - launchPoint.x;
        dir.y = TargetPoint.z - launchPoint.z;
        TargetPoint.y = 0;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float s = launchSpeed;
        float s2 = s * s;
        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        if (r < 0) r = 0;
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        float CosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, SoundData, transform);
        Shell sh = Instantiate(shell);
        sh.Initialize
            (launchPoint
            , TargetPoint
            , new Vector3(s * CosTheta * dir.x, s * sinTheta, s * CosTheta * dir.y)
            , shellBlastRadius
            , shellDamage
            , currentDebuffs
            , unitHitPointPopup
            , Type);
    }
    bool AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, TarggetPoint, hittabelMask);

        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null)
                continue;

            if (targetable.GetFaction() == GetFaction())
                continue;

            float dist = Vector3.Distance(transform.position, targetable.GetTransform().position);
            float priority = targetable.GetTargetPriority();
            float healthPrecent = targetable.GetHealthPrecent();

            float score = priority * 1000f - dist * 2f - (healthPrecent * 200f);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = targetable;
            }
        }

        target = bestTarget;
        return target != null;
    }
    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            OnUpgrade();
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeCrystelAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            shellDamage = Damage[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);
            UpdateCanvasHeight(CurrentLevel);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
            healthBar.SetMaxHealth(currentHP);

        }
    }
}

//Collider[] targets;

//if (!isCaptured)
//{
//    LayerMask[] priorityMasks = {  portalMask, illusionMask, enemyMask };

//    targets = new Collider[0];

//    foreach (var mask in priorityMasks)
//    {
//        targets = Physics.OverlapSphere(transform.position, TarggetPoint, mask);
//        if (targets.Length > 0) break;
//    }
//}
//else
//{
//    targets = Physics.OverlapSphere(transform.position, TarggetPoint, enemyMask);
//}

//if (targets.Length > 0)
//{
//    int ClosestTargetIndex = 0;
//    float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);
//    for (int i = 1; i < targets.Length; i++)
//    {
//        if (MinDist <= MinDist + i)
//        {
//            float dist = Vector3.Distance(transform.position, targets[i].transform.position);
//            if (dist < MinDist)
//            {
//                MinDist = dist;
//                ClosestTargetIndex = i;
//            }
//        }

//    }
//    target = targets[ClosestTargetIndex].GetComponent<Enemy>();
//    if (target != null)
//    {
//        return true;
//    }
//    else
//        return false;
//}
//target = null;
//return false;