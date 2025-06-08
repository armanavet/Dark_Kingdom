using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : Tower
{
    [SerializeField, Range(1, 10f)]
    float TarggetPoint = 2f;
    public LayerMask EnemyMask;
    [SerializeField, Range(1, 100f)]
    float DamagePerSecund = 60f;
    Enemy target;
    [SerializeField] Transform Turret;
    [SerializeField] Arrow Arrow;
    [SerializeField] float arrowSpeed;
    [SerializeField] float attackSpeed;
    float attackCooldown;
    float damage;


    private void Start()
    {
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
        attackCooldown = 1 / attackSpeed;
    }

    void Update()
    {
        if (attackCooldown <= 0)
        { 
            if (AcquireTarget())
            {
                Shoot();
            }
            attackCooldown = 1 / attackSpeed;
        }
        attackCooldown -= Time.deltaTime;

    }
    void Shoot()
    {
        Vector3 point = target.transform.position;
        Turret.LookAt(point);
        float distance = Vector3.Distance(Turret.position, point);
        float arriveTime = distance / arrowSpeed;
        Arrow arrow = Instantiate(Arrow, Turret.position, Quaternion.LookRotation(point - Turret.position));
        arrow.Initialize(arrowSpeed, Turret.position, target,damage);
        StartCoroutine(HitTarget(arriveTime));
        //check level tower
    }
    bool AcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, TarggetPoint, EnemyMask);
        if (targets.Length > 0)
        {
            int ClosestTargetIndex = 0;
            float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);
            for (int i = 1; i < targets.Length; i++)
            {
                if (MinDist <= MinDist + i)
                {
                    float dist = Vector3.Distance(transform.position, targets[i].transform.position);
                    if (dist < MinDist)
                    {
                        MinDist = dist;
                        ClosestTargetIndex = i;
                    }
                }

            }
            target = targets[ClosestTargetIndex].GetComponent<Enemy>();
            if (target != null)
            {
                return true;
            }
            else
                return false;
        }
        target = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 posiion = transform.position;
        Gizmos.DrawWireSphere(posiion, TarggetPoint);
        Gizmos.color = Color.red;
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }

    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            CurrentLevel++;
            Debuff currentDebuff = debuffs[CurrentLevel];
            if (currentDebuff != null)
            {
                foreach (var debuff in currentDebuffs)
                {
                    if(debuff.Type == currentDebuff.Type)
                    {
                        currentDebuffs.Remove(debuff);
                    }
                }
                currentDebuffs.Add(currentDebuff);
            }
            damage = Damage[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];
            UpgradePrice = UpgradePrices[CurrentLevel];
            float hpPercent = currentHP / maxHP;
            currentHP = maxHP * hpPercent;
            maxHP = HP[CurrentLevel];
        }
    }

    IEnumerator HitTarget(float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);
        target.ApplyDamage(damage);
        foreach (var debuff in currentDebuffs)
        {
            DebuffManager.Instance.ApplyDebuff(target, debuff);
        }
        
    }
}
