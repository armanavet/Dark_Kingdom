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
    [SerializeField] Transform targetModel;
    [SerializeField] GameObject fire;

    float TarggetPoint = 2f;
    float rotationProgress;
    float initialRotation;
    bool facingPath = true;

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
        bool targetAcquired = AcquireTarget();
        if (targetAcquired == false)
        {
            if (facingPath) Move();
        }
        else
        {
            facingPath = false;
            DirectionChange turnDirection = CalculateTurnDirection();
            switch (turnDirection)
            {
                case DirectionChange.TurnLeft:
                    animator.SetBool("isTurningLeft", true);
                    break;
                case DirectionChange.TurnRight:
                    animator.SetBool("isTurningRight", true);
                    break;
                case DirectionChange.None:
                    Attack();
                    break;

            }
            initialRotation = transform.eulerAngles.y;
            bool facingTarget = FaceTarget();
            if (facingTarget)
            {
                Attack();
            }
            else
            {
                facingPath = true;
                //StartTurning();
                //target.OnDestroyed -= StartTurning();
                return;
            }
        }
    }
    protected override void Attack()
    {
        animator.SetBool("isTurningLeft", false);
        animator.SetBool("isTurningRight", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
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
    protected override bool AcquireTarget()
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

        GameObject fireObj = null;
        if (target != null)
        {
            fireObj = Instantiate(fire, target.transform.position, Quaternion.identity);
            AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, enemySoundData, target.transform);
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile.gameObject);

        if (fireObj != null)
        {
            yield return new WaitForSeconds(3f);
            Destroy(fireObj);
        }
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
            //model.rotation = Quaternion.Euler(model.rotation.x, yRotation, model.rotation.z);

            return false;
        }
        model.rotation = Quaternion.Euler(model.rotation.x, targetYRotation, model.rotation.z);
        //Attack();
        return true;
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

    void StartTurning()
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
