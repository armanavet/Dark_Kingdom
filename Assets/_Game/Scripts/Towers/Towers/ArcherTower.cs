using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : Tower
{
    [SerializeField] Transform shootingPoint;
    [SerializeField] float projectileSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] AudioClip HitSound;
    [SerializeField, Range(1, 10f)]
    float attackRange = 2f;
    float attackCooldown;
    float damage;
    Enemy target;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        projectile = Projectiles[CurrentLevel];
        if (debuffs.Length != 0 && debuffs[CurrentLevel] != null)
            currentDebuffs.Add(debuffs[CurrentLevel]);
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
        audioSource.clip = shootSound;
        audioSource.PlayOneShot(shootSound);
        Vector3 point = target.transform.position;
        float travelDistance = Vector3.Distance(shootingPoint.position, point);
        float travelTime = travelDistance / projectileSpeed;
        GameObject newProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.LookRotation(point - shootingPoint.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(projectileSpeed);
        StartCoroutine(HitTarget(newProjectile, travelTime));
    }

    bool AcquireTarget()
    {
        Collider[] targets;
        targets = Physics.OverlapSphere(transform.position, attackRange, illusionMask);
        if (targets.Length == 0)
        {
            targets = Physics.OverlapSphere(transform.position, attackRange, enemyMask);
        }
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
        Gizmos.DrawWireSphere(posiion, attackRange);
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
            UpgradePrice = UpgradePrices[CurrentLevel];
            CurrentLevel++;
            SellPrice = SellPrices[CurrentLevel];
            damage = Damage[CurrentLevel];
            projectile = Projectiles[CurrentLevel];
            float hpPercent = currentHP / maxHP;
            currentHP = maxHP * hpPercent;
            maxHP = HP[CurrentLevel];

            Debuff newDebuff = debuffs[CurrentLevel];
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

        if (target != null)
        {
            target.ApplyDamage(damage);
            foreach (var debuff in currentDebuffs)
            {
                DebuffManager.Instance.ApplyDebuff(target, debuff);
            }
        }
        audioSource.clip = HitSound;
        audioSource.PlayOneShot(HitSound);
        Destroy(currentProjectile);
    }
}
