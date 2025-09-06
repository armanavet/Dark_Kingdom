using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainArcherTower : MainTower
{
    [SerializeField] List<Transform> _ShootingPointsList;
    [SerializeField] List<float> _ShootingPointPositions;
    [SerializeField] GameObject _Projectile;
    [SerializeField] float _ProjectileSpeed;
    [SerializeField] float _Damage;
    [SerializeField] float _AttackSpeed;
    [SerializeField, Range(1, 10f)]
    float _AttackRange = 2f;
    float attackCooldown;
    Enemy target;

    private void Start()
    {
        _ShootingPointsList[CurrentLevel].position = new Vector3(0, _ShootingPointPositions[CurrentLevel], 0);
        attackCooldown = 1 / _AttackSpeed;
    }

    private void Update()
    {
        if (attackCooldown <= 0)
        {
            if (AcquireTarget())
            {
                Shoot();
            }
            attackCooldown = 1 / _AttackSpeed;
        }
        attackCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 point = target.transform.position;
        float travelDistance = Vector3.Distance(_ShootingPointsList[CurrentLevel].position, point);
        float travelTime = travelDistance / _ProjectileSpeed;
        GameObject newProjectile = Instantiate(projectile, _ShootingPointsList[CurrentLevel].position, Quaternion.LookRotation(point - _ShootingPointsList[CurrentLevel].position));
        newProjectile.GetComponent<Arrow>()?.Initialize(_ProjectileSpeed);
    }

    bool AcquireTarget()
    {
        Collider[] targets;
        targets = Physics.OverlapSphere(transform.position, _AttackRange, illusionMask);
        if (targets.Length == 0)
        {
            targets = Physics.OverlapSphere(transform.position, _AttackRange, enemyMask);
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
}
    
