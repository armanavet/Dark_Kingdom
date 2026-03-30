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
        SetParameters();
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
        DebuffManager.Instance.RemoveTarget(this);
        WaveManager.Instance.OnEnemyDeath(this);
        StartCoroutine(DestroyObject(Effects));
    }
    public void StartScaling()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length;

        StartCoroutine(ScaleOverTime(new Vector3(2, 2, 2), length));
    }
    IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                time / duration
            );

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
    IEnumerator DestroyObject(GameObject[] effects)
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.EnemyAttack, SoundData, transform);
        gameObject.SetActive(false);
        GameObject effect;
        foreach (var item in effects)
        {
            effect = Instantiate(item, transform.position, Quaternion.identity);
            effectsToDestroy.Add(effect);
        }
        yield return new WaitForSeconds(0.9f);
        foreach (var item in effectsToDestroy)
        {
            Destroy(item.gameObject);
        }
        Destroy(gameObject);
        yield break;
    }
    protected override void Attack() => OnDeath();

}
