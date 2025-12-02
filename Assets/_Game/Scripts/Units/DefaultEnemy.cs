using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    [SerializeField] UnitType unitType;
    void Start()
    {
        currentSpeed = maxSpeed;
        health = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
        //soundData = AudioManager.Instance.GetData(soundData,SFXGroupType.Enemy,UnitType.Regular);
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
        
        attackCooldown -=Time.deltaTime;
        if (target != null&& attackCooldown<=0)
        { 
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
    protected override void PlayAttackSound()
    {
        //AudioManager.Instance.PlayEnemySFX(SoundDataParametor.ClipAttack, unitType, soundData,transform);
    }
    protected override void PlayMovingSound()
    {
        //AudioManager.Instance.PlayEnemySFX(SoundDataParametor.ClipMove, unitType, soundData, transform);

    }
}
