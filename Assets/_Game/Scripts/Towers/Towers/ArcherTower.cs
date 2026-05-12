using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherTower : Tower
{
    [Header("Archer Tower Parameters")]
    [SerializeField] List<float> shootingPointPositions;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float projectileSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField, Range(1, 10f)]
    float attackRange = 2f;


    float attackCooldown;
    float timer;
    float damage;
    ITargetable target;
    public override int GetTargetPriority() => 70;

    private void Start()
    {
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        timer = cooldown;
        shootingPoint.position = new Vector3(transform.position.x, shootingPointPositions[CurrentLevel], transform.position.z);
        projectile = Projectiles[CurrentLevel];
        if (Debuffs[CurrentLevel] != null)
            currentDebuffs.Add(Debuffs[CurrentLevel]);
        currentHP = currentHP == 0 ? maxHP : currentHP;
        healthBar.SetMaxHealth(currentHP);
        attackCooldown = 1 / attackSpeed;
        UpdateCanvasHeight(CurrentLevel);
        towerCanvas.SetActive(false);
        OnPlace();
        SetSoundData();
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (isCaptured)
        {
            HandleCapturedState();
        }

        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Battle)
        {
            return;
        }

        HandleAttack();
    }

    void HandleCapturedState()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy();
        }
    }

    void HandleAttack()
    {
        if (attackCooldown > 0f)
            return;

        if (AcquireTarget())
        {
            timer = cooldown;
            Shoot();
            attackCooldown = 1f / attackSpeed;
        }
    }

    void Shoot()
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, SoundData, transform);
        Vector3 point = target.GetTransform().position;
        float travelDistance = Vector3.Distance(shootingPoint.position, point);
        float travelTime = travelDistance / projectileSpeed;
        GameObject newProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.LookRotation(point - shootingPoint.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(projectileSpeed);
        StartCoroutine(HitTarget(newProjectile, travelTime));
    }
    bool AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, hittabelMask);

        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null)
                continue;

            if (targetable.GetFaction() == this.GetFaction())
                continue;

            float dist = Vector3.Distance(transform.position, targetable.GetTransform().position);
            float priority = targetable.GetTargetPriority();
            float healthPrecent = targetable.GetHealthPrecent();

            float score = priority * 1000f - dist * 2f - (healthPrecent * 200f);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = targetable;
                Debug.Log($"Target: {targetable}, Score: {score}");
            }
        }

        target = bestTarget;
        return target != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 posiion = transform.position;
        Gizmos.DrawWireSphere(posiion, attackRange);
        Gizmos.color = Color.red;
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.GetTransform().position);
        }
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
            SellPrice = SellPrices[CurrentLevel];
            damage = Damage[CurrentLevel];
            projectile = Projectiles[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            shootingPoint.position = new Vector3(transform.position.x, shootingPointPositions[CurrentLevel], transform.position.z);
            model.SetActive(true);
            UpdateCanvasHeight(CurrentLevel);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
            healthBar.SetMaxHealth(currentHP);

            Debuff newDebuff = Debuffs[CurrentLevel];
            if (newDebuff != null)
            {
                foreach (var debuff in currentDebuffs)
                {
                    if (debuff.Type == newDebuff.Type)
                    {
                        currentDebuffs.Remove(debuff);
                        break;
                    }
                }
                currentDebuffs.Add(newDebuff);
            }
        }
    }

    IEnumerator HitTarget(GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, SoundData, currentProjectile.transform);
        if (target != null)
        {
            Enemy enemyTarget = target as Enemy;
            if (enemyTarget != null)
            {
                UIManager.Instance.ShowDamage(unitHitPointPopup, (Enemy)target, damage);
                foreach (var debuff in currentDebuffs)
                {
                    DebuffManager.Instance.ApplyDebuff((Enemy)target, debuff);
                }
            }
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile);
    }
}

//Collider[] targets;

//if (!isCaptured)
//{
//    LayerMask[] priorityMasks = { capturedMask, portalMask, illusionMask, enemyMask };

//    targets = new Collider[0];

//    foreach (var mask in priorityMasks)
//    {
//        targets = Physics.OverlapSphere(transform.position, attackRange, mask);
//        if (targets.Length > 0) break;
//    }
//    Debug.Log($"Captured: {isCaptured}.");
//}
//else
//{
//    targets = Physics.OverlapSphere(transform.position, attackRange, enemyMask);
//    Debug.Log($"Captured: {isCaptured}.");
//}
//Debug.Log("Targets were found - " + targets.Length);
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

//        return true;
//    else
//        return false;
//}
//target = null;
//return false;