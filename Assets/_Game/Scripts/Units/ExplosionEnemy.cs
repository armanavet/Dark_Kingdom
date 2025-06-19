using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [SerializeField]float radius;

    void Start()
    {
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        state = tileFrom.isEmpty && help>0 ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }
    protected override void Attack() => OnDeath();
  
    protected override void OnDeath()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, towerMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<Tower>().ApplyDamage(damage);
            }
        }
        WaveManager.Instance.OnEnemyDeath();
        Destroy(gameObject);
    }
}
