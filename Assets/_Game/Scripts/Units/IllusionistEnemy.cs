using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IllusionistEnemy : Enemy
{
    [SerializeField] Illusion illusionPrefab;
    [SerializeField] float detectionRange;
    [SerializeField] float illusionSpawnTime;
    float illusionCooldown;
    Illusion illusion;

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
    void Update()
    {
        if (state == EnemyState.Dead) return;

        if (illusionCooldown <= 0)
        {
            if (DetectTowers())
            {
                SpawnIllusion();
                illusionCooldown = illusionSpawnTime;
            }
        }
        illusionCooldown -= Time.deltaTime;
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }

    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
    bool DetectTowers()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, towerMask);
        return targets.Length > 0;
    }
    void SpawnIllusion()
    {
        if (illusion == null)
        {
            illusion = Instantiate(illusionPrefab, transform.position, transform.rotation);
            illusion.OnSpawn(tileFrom, 0);
        }
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
