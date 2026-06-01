using UnityEngine;

public class DefaultEnemy : Enemy
{
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
    protected override void Attack()
    {
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsAttacking, true);
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            target.ApplyDamage(damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
}
