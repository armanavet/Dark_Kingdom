using UnityEngine;
using System.Collections;
using AudioSystem;
using UnityEditor.Experimental.GraphView;

public class FlyingEnemy : Enemy
{
    [Header("Flying Enemy Parameters")]
    [SerializeField] Mage mage;
    [SerializeField] float height;
    [SerializeField] float rotationSpeed;
    [SerializeField] float flyUpSpeed;
    [SerializeField] float distanceToAttack;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform shootingPoint;

    Vector3 targetPoint;
    Quaternion targetRotation;
    bool isAttacking;
    float rotationProgress;
    void Start()
    {
        SetParameters();
        Spawn();
    }
    void Update()
    {
        if (state == EnemyState.Dead)
        {
            Fall();
            return;
        }
        if (attackCooldown > 0)
        {
            Debug.Log("1");
            attackCooldown -= Time.deltaTime;
        }
        if (isAttacking == true) return;
        if (Vector3.Distance(transform.position, targetPoint) > distanceToAttack)
        {
            Move();
            return;
        }
        HandleAttack();
    }
    protected override void Move()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
    protected override void Attack()
    {
        Debug.Log("3");
        isAttacking = true;
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", true);
    }
    void HandleAttack()
    {
        if (attackCooldown > 0) return;
        Debug.Log("2");
        Attack();
    }
    public void LaunchProjectile()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            float travelDistance = Vector3.Distance(shootingPoint.position, targetPosition);
            float travelTime = travelDistance / projectileSpeed;
            Mage fireBall = Instantiate(mage, shootingPoint.position, Quaternion.LookRotation(targetPosition - transform.position));
            fireBall.Initialize(projectileSpeed);
            StartCoroutine(HitTarget(fireBall, travelTime));
        }
    }
    public void OnAttackFinished()
    {
        isAttacking = false;
        attackCooldown = 1 / attackSpeed;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", true);
        Debug.Log("4");
    }
    void Spawn()
    {
        target = TowerManager.Instance.Towers[0];
        targetPoint = target.transform.position + new Vector3(0, height, 0);
        targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

        transform.position = new Vector3(transform.position.x, height, transform.position.z);

        transform.rotation = Quaternion.Euler(transform.rotation.x, targetRotation.eulerAngles.y, transform.rotation.z);

    }
    void Fall()
    {
        if (transform.position.y <= 0) return;
        transform.Translate(-transform.up * flyUpSpeed * Time.deltaTime);
    }

    IEnumerator HitTarget(Mage currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        if (target != null)
        {
            AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, SoundData, transform);
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile.gameObject);
    }
}

//bool FaceTarget()
//{
//    float rotationDifference = targetRotation.eulerAngles.y - transform.rotation.y;
//    float rotationTime = rotationDifference / rotationSpeed;
//    rotationProgress += Time.deltaTime / rotationTime;
//    if (rotationProgress >= 1) return false;
//    float yRotation = Mathf.LerpAngle(transform.rotation.y, targetRotation.eulerAngles.y, rotationProgress);
//    return true;
//}
//bool FlyUp()
//{

//    if (transform.position.y >= height) return false;
//    transform.Translate(transform.up * flyUpSpeed * Time.deltaTime);
//    return true;
//}