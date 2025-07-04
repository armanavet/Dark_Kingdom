using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MageEnemy : Enemy
{
    float TarggetPoint = 2f;
    [SerializeField] Mage mage;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (state == EnemyState.Dead) return;
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
        //if (AcquireTarget()) Attack();
        //else Move();
    }
    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            Vector3 targetPosition = target.transform.position;
            Mage arrow = Instantiate(mage, transform.position, Quaternion.LookRotation(targetPosition - transform.position));
            arrow.Initialize(attackSpeed, transform.position, targetPosition, target, damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
  
    protected override bool AcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, TarggetPoint, towerMask );
        if (targets.Length > 0)
        {
            int ClosestTargetIndex = 0;
            float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);
            for (int i = 1; i < targets.Length; i++)
            {
                if (MinDist <= MinDist + i)
                {
                    float dist = Vector3.Distance(transform.position, targets[i].transform.position);
                    if (dist < MinDist)
                    {
                        MinDist = dist;
                        ClosestTargetIndex = i;
                    }
                }

            }
            target = targets[ClosestTargetIndex].GetComponentInChildren<Tower>();
            if (target != null)
            {
                return true;
            }
            else
                return false;
        }
        target = null;
        return false;
    }
    public void PlayAttackSound()
    {
        audioSource.clip = attackSound;
        audioSource.PlayOneShot(attackSound);
    }
    public void PlayWalkingSound()
    {
        int randomSound = Random.Range(0, movingSounds.Length);
        audioSource.clip = movingSounds[randomSound];
        audioSource.PlayOneShot(movingSounds[randomSound]);
    }
}
