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
        //soundData = AudioManager.Instance.SetEnemySFXData(UnitType.Kamikadze, soundData);
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
        //AudioManager.Instance.PlayExplosionSound(EnemyAttackSound, transform);
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, towerMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<Tower>().ApplyDamage(damage);
            }
        }
        WaveManager.Instance.OnEnemyDeath(this);
        //foreach (var effect in Effects)
        //{
        //    Instantiate(effect, transform.position, Quaternion.identity);
        //    Destroy(effect);
        //}
        //Destroy(gameObject);
        StartCoroutine(DestroyObject(Effects));
    }

    protected override void Attack() => OnDeath();
    IEnumerator DestroyObject(GameObject[] effects)
    {
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

    protected override void PlayAttackSound()
    {
        //AudioManager.Instance.PlayEnemySFX(SoundDataParametor.ClipAttack, UnitType.Kamikadze, soundData, transform);
    }
    protected override void PlayMovingSound()
    {
        //AudioManager.Instance.PlayEnemySFX(SoundDataParametor.ClipMove, UnitType.Kamikadze, soundData, transform);

    }
}
