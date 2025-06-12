using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloneEnemy : Enemy
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
}
