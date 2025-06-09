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
    }
    void Update()
    {
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }
    protected override void Attack()
    {
        if (target == null)
        {
        }
        else
        {
            target.ApplyDamage(damage);
        }
    }
}
