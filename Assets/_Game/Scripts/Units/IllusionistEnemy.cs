using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IllusionistEnemy : Enemy
{
    [Header("Illusionist Enemy Parameters")]
    [SerializeField] Illusion illusionPrefab;
    [SerializeField] float detectionRange;
    [SerializeField] float illusionSpawnTime;
    float illusionCooldown;
    Illusion illusion;
    public override int GetTargetPriority() => 40;

    void Start()
    {
        SetParameters();
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;

        if (illusionCooldown <= 0)
        {
            if (DetectTowers())
            {
                SpawnIllusion();
                illusionCooldown = illusionSpawnTime;
            }
        }
        illusionCooldown -= Time.deltaTime;
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }

    protected override void Attack()
    {
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsAttacking, true);
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
    bool DetectTowers()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, towerMask);
        return targets.Length > 0;
    }
    void SpawnIllusion()
    {
        if (illusion == null)
        {
            illusion = Instantiate(illusionPrefab, transform.position, transform.rotation);
        }
    }
}
