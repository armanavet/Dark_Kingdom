using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [Header("Explosion Enemy Parameters")]
    [SerializeField]float radius;
    [SerializeField]GameObject[] Effects;
    private void Awake()
    {
        if (enemyAudioSource == null)
        {
            enemyAudioSource = GetComponent<AudioSource>();
        }
    }
    void Start()
    {
        currentSpeed = maxSpeed;
        health = maxHP;
        damage = maxDamage;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (state == EnemyState.Dead) return;
        state = tileFrom.isEmpty ? EnemyState.Moving : EnemyState.Attacking;
        if (state == EnemyState.Moving) Move();
        else if (state == EnemyState.Attacking) Attack();
    }

    private void Explode()
    {
        AudioManager.instance.PlayExplosionSound(EnemyAttackSound);
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, towerMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<Tower>().ApplyDamage(damage);
            }
        }
        WaveManager.Instance.OnEnemyDeath(this);
        foreach (var effect in Effects) 
        {
            Instantiate(effect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    protected override void Attack() => OnDeath();  
    
}
