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
        //soundData = AudioManager.Instance.SetEnemySFXData(UnitType.Illusionist, soundData);
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;

        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving)
        {
            animator.SetBool("isIdle", false);
            Move();
        }
        else
        {
            animator.SetBool("isIdle", true);
        }
    }
    protected override void Attack()
    {
    }
    protected override void PlayAttackSound() { }
    protected override void PlayMovingSound()
    {
        //AudioManager.Instance.PlayEnemySFX(SoundDataParametor.ClipMove, UnitType.Illusionist, soundData, transform);
    }
}
