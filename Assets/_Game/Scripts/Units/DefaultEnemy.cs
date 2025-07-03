using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        Debug.Log(audioSource);
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }
    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
        audioSource.clip = attackSound; // stop run loop if playing
        audioSource.PlayOneShot(attackSound);
        attackCooldown -=Time.deltaTime;
        if (target != null&& attackCooldown<=0)
        { 
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
}
