using UnityEngine;

public class IllusionistEnemy : Enemy
{
    [Header("Illusionist Enemy Parameters")]
    [SerializeField] Illusion illusionPrefab;
    [SerializeField] float illusionSpawnTime;
    float illusionCooldown;
    Illusion illusion;

    void Start()
    {
        SetParameters();
    }

    void Update()
    {
        if (state == EnemyState.Dead) return;

        illusionCooldown -= Time.deltaTime;
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

    public Illusion OnDetected(out bool targetIllusion)
    {
        targetIllusion = SpawnIllusion();
        return illusion;
    }

    private bool SpawnIllusion()
    {
        if (illusion != null) return false;
        if (illusionCooldown > 0) return false;

        illusion = Instantiate(illusionPrefab, transform.position, transform.rotation);
        illusion.Model.position = model.position;
        illusion.Model.rotation = model.rotation;
        illusionCooldown = illusionSpawnTime;
        return true;
    }
}
