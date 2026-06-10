using System.Linq;
using UnityEngine;

public class ArtilleryTower : Tower
{
    [Header("Artillery Tower Parameters")]
    [SerializeField] Transform shootingPoint;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1;

    private ITargetable target;
    private float g => Mathf.Abs(Physics.gravity.y);
    private float launchSpeed;
    private float timer;
    private bool canAttack = true;

    void Awake()
    {
        float x = range + 0.250001f;
        float y = -shootingPoint.position.y;
        launchSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
    }

    void Update()
    {
        timer -= Time.deltaTime;
        CheckStrategy();

        if (canAttack && timer <= 0 && AcquireTarget())
        {
            Launch(target);
            timer = cooldown;
        }
    }

    void Launch(ITargetable target)
    {
        if (target == null)
        {
            return;
        }
        Vector2 dir;
        Vector3 launchPoint = shootingPoint.position;
        Vector3 TargetPoint = target.Transform.position;
        dir.x = TargetPoint.x - launchPoint.x;
        dir.y = TargetPoint.z - launchPoint.z;
        TargetPoint.y = 0;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float s = launchSpeed;
        float s2 = s * s;
        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        if (r < 0) r = 0;
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        float CosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, soundData, transform);
        Shell sh = Instantiate(projectile).GetComponent<Shell>();

        if (sh == null)
        {
            Debug.LogError($"Attach a Shell script to {name} projectile!");
            return;
        }

        sh.Initialize
            (launchPoint
            , TargetPoint
            , new Vector3(s * CosTheta * dir.x, s * sinTheta, s * CosTheta * dir.y)
            , shellBlastRadius
            , damage
            , currentDebuffs.Values.ToList()
            , Type);
    }

    bool AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, TargetMask);

        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null)
                continue;

            if (targetable.Faction == Faction)
                continue;

            float dist = Vector3.Distance(transform.position, targetable.Transform.position);
            float priority = targetable.TargetPriority;
            float healthPrecent = targetable.HealthPercent;

            float score = priority * 1000f - dist * 2f - (healthPrecent * 200f);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = targetable;
            }
        }

        target = bestTarget;
        return target != null;
    }

    protected override void CheckStrategy()
    {
        if (StrategyManager.Instance.CurrentStrategy == StrategyType.Battle)
        {
            canAttack = true;
            foreach (var effect in sleepFX)
            {
                effect.SetActive(false);
            }
        }
        else
        {
            canAttack = false;
            foreach (var effect in sleepFX)
            {
                effect.SetActive(true);
            }
        }
    }
}
