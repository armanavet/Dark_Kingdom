using AudioSystem;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
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
    Tween faceTargetTween, facePathTween;
    bool facingTarget, facingPath = true;

    public override int GetTargetPriority() => 40;

    private void OnDisable()
    {
        if (target != null)
        {
            target.OnDestroyed -= FacePath;
        }
    }

    void Start()
    {
        SetParameters();
    }
    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (AcquireTargets())
        {
            facingPath = false;
            if (facingTarget && attackCooldown <= 0)
            {
                Attack();
                attackCooldown = 1 / attackSpeed;
            }
        }
        else
        {
            facingTarget = false;
            if (facingPath) Move();
        }
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
            Mage arrow = Instantiate(mage, shootingPoint.position, shootingPoint.rotation);
            arrow.Initialize(projectileSpeed);
            StartCoroutine(HitTarget(arrow, travelTime));
        }
    }

    protected override bool AcquireTargets()
    {
        if (target != null && !target.IsDestroyed) return true;

        target = null;
        facingTarget = false;
        Collider[] targets = Physics.OverlapSphere(transform.position, TarggetPoint, towerMask);
        if (targets.Length > 0)
        {
            float MinDist = float.MaxValue;
            for (int i = 0; i < targets.Length; i++)
            {
                if (!targets[i].TryGetComponent(out Tower tower) || tower.IsDestroyed) continue;

                float dist = Vector3.Distance(transform.position, tower.transform.position);
                if (dist < MinDist)
                {
                    MinDist = dist;
                    target = tower;
                }
            }
            if (target != null)
            {
                target.OnDestroyed += FacePath;
                FaceTarget();
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void FaceTarget()
    {
        model.DOKill();
        model.DOLookAt(target.transform.position, 1 / rotationSpeed, AxisConstraint.Y).SetAutoKill(false).OnComplete(() => facingTarget = true);
    }

    private void FacePath(Tower _)
    {
        target.OnDestroyed -= FacePath;
        model.DOLocalRotate(Vector3.zero, 1 / rotationSpeed).SetAutoKill(false).OnComplete(() => facingPath = true);
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

    DirectionChange CalculateTurnDirection()
    {
        float targetYRotation = Quaternion.LookRotation(target.transform.position - model.position).eulerAngles.y;
        float currentRotation = model.eulerAngles.y;
        if (targetYRotation < currentRotation) return DirectionChange.TurnLeft;
        else if (targetYRotation > currentRotation) return DirectionChange.TurnRight;
        else return DirectionChange.None;
    }
}