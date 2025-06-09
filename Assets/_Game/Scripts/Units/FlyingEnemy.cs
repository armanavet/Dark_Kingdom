using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] float height;
    void Start()
    {
        transform.position += new Vector3(0, height, 0);
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
    protected override void Move()
    {
        

    }
    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
