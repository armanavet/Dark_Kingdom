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
        EnemySoundData = AudioManager.Instance.SetData(EnemySoundData, SoundDataFor.Enemy, MixerFor.Enemy, unitType);
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
    protected override void Attack() { }
}
