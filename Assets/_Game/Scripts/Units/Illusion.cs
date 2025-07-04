using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illusion : Enemy
{
    [SerializeField] float duration;
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        currentSpeed = maxSpeed;
        help = maxHP;
        animator = GetComponent<Animator>();
        Destroy(gameObject,duration);
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
    protected override void Attack(){
    }
    //public void PlayGrowelSound()
    //{
    //    audioSource.clip = attackSound;
    //    audioSource.PlayOneShot(attackSound);
    //}
    //public void PlayWalkingSound()
    //{
    //    int randomSound = Random.Range(0, movingSounds.Length);
    //    audioSource.clip = movingSounds[randomSound];
    //    audioSource.PlayOneShot(movingSounds[randomSound]);
    //}
}
