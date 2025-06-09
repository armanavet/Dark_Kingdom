using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDebuffable
{
    [SerializeField] Transform model;
    [SerializeField] protected LayerMask towerMask;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float maxDamage;
    protected float currentSpeed;
    protected float help;
    protected float damage;
    protected Tile tileFrom, tileTo;
    protected EnemyState state;
    protected Tower target;
    Vector3 positionFrom, positionTo;
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    float progress, progressFactor;
    float positionOffset;
    public Vector3 CurrentPosition => model.position;
    public void OnSpawn(Tile startingTile, float positionOffset)
    {
        tileFrom = startingTile;
        tileTo = tileFrom.NextOnPath;
        this.positionOffset = positionOffset;
        PrepareInitialMove();
        progress = 0;
    }

    void PrepareInitialMove()
    {
        positionFrom = tileFrom.transform.position;
        positionTo = tileFrom.exitPoint;
        direction = tileFrom.pathDirection;
        directionChange = DirectionChange.None;
        model.localPosition = new Vector3(positionOffset, 0, 0);
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2;
    }

    protected virtual void Move()
    {
        progress += Time.deltaTime * progressFactor * currentSpeed;
        if (progress > 1)
        {
            tileFrom = tileTo;
            tileTo = tileFrom.NextOnPath;
            progress = 0;
            PrepareNextMove();
        }
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.Lerp(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }

    void PrepareNextMove()
    {
        model.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        positionFrom = positionTo;
        positionTo = tileFrom.exitPoint;
        directionChange = direction.ChangeDirectionTo(tileFrom.pathDirection);
        direction = tileFrom.pathDirection;
        directionAngleFrom = directionAngleTo;
        AcquireTarget();
        switch (directionChange)
        {
            case DirectionChange.None: PrepareMoveForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }
    void PrepareMoveForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleFrom = direction.GetAngle();
        model.localPosition = new Vector3(positionOffset, 0, 0);
        progressFactor = 1;
    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90;
        model.localPosition = new Vector3(positionOffset - 0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1 / (Mathf.PI * 0.5f * (0.5f - positionOffset));
    }
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90;
        model.localPosition = new Vector3(positionOffset + 0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1 / (Mathf.PI * 0.5f * (0.5f - positionOffset));
    }
    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180;
        model.localPosition = new Vector3(positionOffset - 0.5f, 0, 0);
        transform.localPosition = positionFrom;
    }

    protected abstract void Attack();
    protected virtual bool AcquireTarget()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, towerMask))
        {
            target = hitInfo.transform.GetComponent<Tower>();
           
        } 
        return true;
    }
    public void ApplyDamage(float damage)
    {
        help -= damage;
        if (help <= 0)
        {
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {
        WaveManager.Instance.OnEnemyDeath();
        Destroy(gameObject);
    }
    public void ApplySlow(float slow)
    {
        currentSpeed = maxSpeed * (1 - slow);
    }
}

public enum EnemyState
{
    Attacking,
    Moving
}
public enum UnitType
{
   Regular,
   Fast,
   Tank,
   Mage,
   Kamikadze,
   Flying,
   Clone
}
