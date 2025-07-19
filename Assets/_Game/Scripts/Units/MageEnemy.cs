using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MageEnemy : Enemy
{
    float TarggetPoint = 2f;
    float rotationProgress;
    [SerializeField] float rotationSpeed;
    [SerializeField] Mage mage;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform targetModel;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        bool targetAcquired = AcquireTarget();
        if (targetAcquired == false) 
        { 
            Move();
        }
        else
        {
            animator.SetBool("isMoving", false);
            bool facingTarget = FaceTarget();
            if (facingTarget) Attack();
        }
        
    }
    protected override void Attack()
    {
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
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile.gameObject);
    }

    bool FaceTarget()
    {
        if (rotationProgress < 1)
        {
            //float targetYRotation = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles.y;
            //float rotationDifference = targetYRotation - transform.rotation.eulerAngles.y;
            //float rotationTime = rotationDifference / rotationSpeed;
            //rotationProgress += Time.deltaTime / rotationTime;
            //float yRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetYRotation, rotationProgress);
            //transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);
            //return false;

            float targetYRotation = Quaternion.LookRotation(targetModel.position - transform.position).eulerAngles.y;
            float rotationDifference = targetYRotation - targetModel.eulerAngles.y;
            float rotationTime = rotationDifference / rotationSpeed;
            rotationProgress += Time.deltaTime / rotationTime;
            float yRotation = Mathf.LerpAngle(targetModel.eulerAngles.y, targetYRotation, rotationProgress);
            transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);
            return false;
        }
        rotationProgress = 0;
        return true;
    }
}
