using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illusion : Enemy
{
    [Header("Illusion Parameters")]
    [SerializeField] float duration;

    void Start()
    {
        currentSpeed = maxSpeed;
        health = maxHP;
        animator = GetComponent<Animator>();
        Destroy(gameObject, duration);
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;
    }
    protected override void Attack() { }
}
