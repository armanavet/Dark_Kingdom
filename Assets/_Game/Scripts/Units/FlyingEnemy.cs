using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] float height;
    Vector3 targetPoint;
    float movementProgress;
    float TarggetPoint = 2f;
    [SerializeField] Mage mage;
    void Start()
    {
        transform.position += new Vector3(0, height, 0);
        target = TowerManager.Instance.Towers[0];
        targetPoint = target.transform.position + new Vector3(0, height, 0);
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
    }
    void Update()
    {
        movementProgress += Time.deltaTime * currentSpeed;
        if (movementProgress >= 1) Attack();
        else Move();
    }
    protected override void Move()
    {
        transform.position = Vector3.Lerp(CurrentPosition, targetPoint, movementProgress);
    }
    protected override void Attack()
    {
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            Mage arrow = Instantiate(mage, transform.position, Quaternion.LookRotation(targetPoint - transform.position));
            arrow.Initialize(attackSpeed, transform.position, targetPoint, target, damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
}
