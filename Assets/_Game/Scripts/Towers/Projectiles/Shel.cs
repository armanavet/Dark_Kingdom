using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shel : MonoBehaviour
{
    Vector3 launchPoint, targetPoint, launchVelocity;
    float age, blastRadius, damage;
    List<Debuff> debuffs;
    public LayerMask EnemyMask;
    
    void Update()
    { 
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.position = p;
        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        if (transform.position.y < 0f)
        {
            Explode();

        }

    }
    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float blastRadius, float damage, List<Debuff> debuffs)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
        this.debuffs = debuffs;
    }
    void Explode()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, blastRadius, EnemyMask);

        if (targets.Length > 0)
        {
            foreach(var target in targets)
            {
                Enemy enemy = target.GetComponent<Enemy>();
                enemy.ApplyDamage(damage);
                foreach (var debuff in debuffs)
                {
                    DebuffManager.Instance.ApplyDebuff(enemy, debuff);
                }
            }
        }
        Destroy(gameObject);
    }

}
