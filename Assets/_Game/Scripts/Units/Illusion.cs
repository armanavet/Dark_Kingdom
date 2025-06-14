using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illusion : Enemy
{
    [SerializeField] float duration;
    void Start()
    {
        currentSpeed = maxSpeed;
        help = maxHP;
        Destroy(gameObject,duration);
    }
    void Update()
    {
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
    }
    protected override void Attack(){}
}
