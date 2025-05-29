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
    TargetPoint target;
    [SerializeField] Transform Turret;
    [SerializeField] Arrow Arrow;
    [SerializeField] float arrowSpeed;
    [SerializeField] float attackSpeed;
    float attackCooldown;

    private void Start()
    {
        attackCooldown=1/attackSpeed;
    }
    // Update is called once per frame
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
        float d = Vector3.Distance(Turret.position, point);
        Arrow arrow = Instantiate(Arrow, Turret.position, Quaternion.LookRotation(point - Turret.position));
        arrow.Initialize(arrowSpeed, Turret.position, point);
        
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
            target = targets[ClosestTargetIndex].GetComponent<TargetPoint>();
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
        if (levelOFTower < SellPrices.Count - 1 && levelOFTower < UpgradePrices.Count)
        {
            SellPrice = SellPrices[levelOFTower + 1];
            UpgradePrice = UpgradePrices[levelOFTower];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);
            levelOFTower++;
        }
    }
}
