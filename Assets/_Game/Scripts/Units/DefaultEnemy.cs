using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    void Start()
    {
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
    }
    void Update()
    {
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }
    protected override void Attack()
    {
        attackCooldown-=Time.deltaTime;
        if (target != null&& attackCooldown<=0)
        { 
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
}
