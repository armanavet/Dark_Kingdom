using UnityEngine;
using System.Collections;

public class FlyingEnemy : Enemy
{
    [SerializeField] Mage mage;
    [SerializeField] float height;
    [SerializeField] float rotationSpeed;
    [SerializeField] float flyUpSpeed;
    [SerializeField] float distanceToAttack;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform shootingPoint;
    Vector3 targetPoint;
    Quaternion targetRotation;
    float rotationProgress;
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        target = TowerManager.Instance.Towers[0];
        targetPoint = target.transform.position + new Vector3(0, height, 0);
        targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (state == EnemyState.Dead) 
        {
            Fall(); 
            return; 
        } 
        if (FlyUp()) return;
        else if (FaceTarget()) return;
        else if (Vector3.Distance(transform.position, targetPoint) > distanceToAttack) Move();
        else Attack();
        
    }
    protected override void Move()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isAttacking", false);
        transform.Translate(Vector3.forward * currentSpeed*Time.deltaTime);
    }
    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
    }

    public void LaunchProjectile()
    {
        if (target != null && attackCooldown <= 0)
        {
            Vector3 targetPosition = target.transform.position;
            float travelDistance = Vector3.Distance(shootingPoint.position, targetPosition);
            float travelTime = travelDistance / projectileSpeed;
            Mage arrow = Instantiate(mage, shootingPoint.position, Quaternion.LookRotation(targetPosition - transform.position));
            arrow.Initialize(projectileSpeed);
            StartCoroutine(HitTarget(arrow, travelTime));
        }
        
    }
    bool FaceTarget()
    {
        float rotationDifference = targetRotation.eulerAngles.y - transform.rotation.y;
        float rotationTime=rotationDifference/rotationSpeed;
        rotationProgress += Time.deltaTime / rotationTime;
        if(rotationProgress>=1)return false;
        float yRotation=Mathf.LerpAngle(transform.rotation.y,targetRotation.eulerAngles.y,rotationProgress);
        transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);
        return true;
    }
    bool FlyUp()
    {
        if (transform.position.y>=height) return false;
        transform.Translate(transform.up * flyUpSpeed*Time.deltaTime);
        return true;
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
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile.gameObject);
    }
}
