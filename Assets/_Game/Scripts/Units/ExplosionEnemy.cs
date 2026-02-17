using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [Header("Explosion Enemy Parameters")]
    [SerializeField] float radius;
    [SerializeField] GameObject[] Effects;
    List<GameObject> effectsToDestroy = new List<GameObject>();
    void Start()
    {
        currentSpeed = maxSpeed;
        health = maxHP;
        damage = maxDamage;
        animator = GetComponent<Animator>();
        //EnemySoundData = AudioManager.Instance.SetData(EnemySoundData, SoundDataType.Enemy, MixerType.Enemy, unitType);
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
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, towerMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<Tower>().ApplyDamage(damage);
            }
        }
        WaveManager.Instance.OnEnemyDeath(this);
        StartCoroutine(DestroyObject(Effects));
    }

    protected override void Attack() => OnDeath();
    IEnumerator DestroyObject(GameObject[] effects)
    {
        //AudioManager.Instance.Play(EnemySoundData, transform, ClipType.OnAttack_Enemy);
        yield return new WaitForSeconds(1.5f);
        GameObject effect;
        foreach (var item in effects)
        {
            effect = Instantiate(item, transform.position, Quaternion.identity);
            effectsToDestroy.Add(effect);
        }
        yield return new WaitForSeconds(0.9f);
        gameObject.SetActive(false);
        foreach (var item in effectsToDestroy)
        {
            Destroy(item.gameObject);
        }
        Destroy(gameObject);
        yield break;
    }

}
