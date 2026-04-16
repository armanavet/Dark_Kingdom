using AudioSystem;
using System.Collections;
using UnityEngine;

public class MageEnemy : Enemy
{
    [Header("Mage Enemy Parameters")]
    [SerializeField] float rotationSpeed;
    [SerializeField] Mage mage;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject fire;

    float TarggetPoint = 2f;
    float rotationProgress;
    float initialRotation;
    bool facingPath = true;
    public override int GetTargetPriority() => 40;

    private void OnDestroy()
    {
        if (target != null)
        {
            target.OnDestroyed -= StartTurning;
        }
    }
    void Start()
    {
        SetParameters();
    }
    private void Update()
    {
        bool targetAcquired = AcquireTargets();
        if (targetAcquired == false)
        {
            if (facingPath) Move();
        }
        else
        {
            if (attackCooldown <= 0)
            {
                facingPath = false;
                initialRotation = transform.eulerAngles.y;
                bool facingTarget = FaceTarget();
                if (facingTarget)
                {
                    Attack();
                    attackCooldown = 1 / attackSpeed;
                }
                else
                {
                    facingPath = true;
                    return;
                }
            }
        }
        attackCooldown -= Time.deltaTime;
    }
    protected override void Attack()
    {
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsAttacking, true);
    }
    public void LaunchProjectile()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            float travelDistance = Vector3.Distance(shootingPoint.position, targetPosition);
            float travelTime = travelDistance / projectileSpeed;
            Mage arrow = Instantiate(mage, shootingPoint.position, Quaternion.LookRotation(targetPosition - transform.position));
            arrow.Initialize(projectileSpeed);
            StartCoroutine(HitTarget(arrow, travelTime));
        }
    }
    protected override bool AcquireTargets()
    {
        if (target != null) return true;
        Collider[] targets = Physics.OverlapSphere(transform.position, TarggetPoint, towerMask);
        if (targets.Length > 0)
        {
            int ClosestTargetIndex = 0;
            float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);
            for (int i = 1; i < targets.Length; i++)
            {
                if (MinDist <= MinDist + i)
                {
                    float dist = Vector3.Distance(transform.position, targets[i].transform.position);
                    if (dist < MinDist)
                    {
                        MinDist = dist;
                        ClosestTargetIndex = i;
                    }
                }
            }
            target = targets[ClosestTargetIndex].GetComponentInChildren<Tower>();
            if (target != null)
            {
                target.OnDestroyed += StartTurning;
                rotationProgress = 0;
                return true;
            }
            else
                return false;
        }
        target = null;
        return false;
    }
    IEnumerator HitTarget(Mage currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        if (target != null)
        {
            AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, enemySoundData, target.transform);
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile.gameObject);
    }

    bool FaceTarget()
    {
        float targetYRotation = Quaternion.LookRotation(target.transform.position - model.position).eulerAngles.y;
        if (rotationProgress < 1)
        {
            float rotationDifference = targetYRotation - target.transform.eulerAngles.y;
            float rotationTime = rotationDifference / rotationSpeed;
            rotationProgress += Time.deltaTime / rotationTime;
            float yRotation = Mathf.LerpAngle(model.eulerAngles.y, targetYRotation, rotationProgress);
            model.rotation = Quaternion.Euler(model.rotation.x, yRotation, model.rotation.z);
            return true;
        }
        return false;
    }
    IEnumerator FacePath()
    {
        float targetYRotation = initialRotation;
        float rotationDifference = targetYRotation - target.transform.eulerAngles.y;
        float rotationTime = rotationDifference / rotationSpeed;
        rotationProgress += Time.deltaTime / rotationTime;
        float yRotation = Mathf.LerpAngle(model.eulerAngles.y, targetYRotation, rotationProgress);
        model.rotation = Quaternion.Euler(model.rotation.x, yRotation, model.rotation.z);
        yield return new WaitUntil(() => rotationProgress >= 1);
        facingPath = true;
    }

    void StartTurning(Tower target)
    {
        target.OnDestroyed -= StartTurning;
        StartCoroutine(FacePath());
    }

    DirectionChange CalculateTurnDirection()
    {
        float targetYRotation = Quaternion.LookRotation(target.transform.position - model.position).eulerAngles.y;
        float currentRotation = model.eulerAngles.y;
        if (targetYRotation < currentRotation) return DirectionChange.TurnLeft;
        else if (targetYRotation > currentRotation) return DirectionChange.TurnRight;
        else return DirectionChange.None;
    }
}