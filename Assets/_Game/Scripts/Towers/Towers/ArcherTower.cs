using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : Tower
{
    [Header("Archer Tower Parameters")]
    [SerializeField] Transform shootingPoint;
    [SerializeField] List<float> shootingPointPositions;
    [SerializeField] float projectileSpeed;
    private ITargetable target;
    private float timer;

    //private void Start()
    //{
    //    soundData = AudioManager.Instance.SetData(Type, SoundDataType.Gameplay);
    //    UpdateData();
    //    OnPlace();
    //}

    void Update()
    {
        timer -= Time.deltaTime;

        //if (IsCaptured)
        //{
        //    HandleCapturedState();
        //}

        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Battle) return;

        if (timer <= 0 && AcquireTarget())
        {
            timer = cooldown;
            Shoot();
        }
    }

    //void HandleCapturedState()
    //{
    //    timer -= Time.deltaTime;

    //    if (timer <= 0f)
    //    {
    //        Destroy();
    //    }
    //}

    void Shoot()
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, soundData, transform);
        Vector3 point = target.Transform.position;
        float travelDistance = Vector3.Distance(shootingPoint.position, point);
        float travelTime = travelDistance / projectileSpeed;
        GameObject newProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.LookRotation(point - shootingPoint.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(projectileSpeed);
        StartCoroutine(HitTarget(newProjectile, travelTime));
    }

    bool AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, TargetMask);

        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null)
                continue;

            if (targetable.Faction == Faction)
                continue;

            float dist = Vector3.Distance(transform.position, targetable.Transform.position);
            float priority = targetable.TargetPriority;
            float healthPrecent = targetable.HealthPercent;

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
        Gizmos.DrawWireSphere(posiion, range);
        Gizmos.color = Color.red;
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.Transform.position);
        }
    }

    protected override void UpdateData()
    {
        base.UpdateData();

        shootingPoint.position = new Vector3(transform.position.x, shootingPointPositions[currentLevel - 1], transform.position.z);
    }

    IEnumerator HitTarget(GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, soundData, currentProjectile.transform);
        if (target != null)
        {
            Enemy enemyTarget = target as Enemy;
            if (enemyTarget != null)
            {
                UIManager.Instance.ShowDamage(enemyTarget, damage);
                foreach (var debuff in currentDebuffs)
                {
                    DebuffManager.Instance.ApplyDebuff((Enemy)target, debuff.Value);
                }
            }
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile);
    }
}
