using System.Linq;
using UnityEngine;

public class WizardTower : Tower
{
    [Header("Wizard Tower Parameters")]
    [SerializeField] Transform shootingPoint;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1;

    private ITargetable target;
    private float g => Mathf.Abs(Physics.gravity.y);
    private float launchSpeed;
    private float timer;
    private bool canAttack = true;

    private void Awake()
    {
        float x = range + 0.250001f;
        float y = -shootingPoint.position.y;
        launchSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
    }

    void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;

        timer -= Time.deltaTime;
        CheckStrategy();
        AcquireTarget();
        if (target == null || target.IsDead) return;

        if (canAttack && timer <= 0)
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

    private void AcquireTarget()
    {
        if (target != null && !target.IsDead) return;

        float minDist = float.MaxValue;
        Collider[] hits = Physics.OverlapSphere(transform.position, range, TargetMask);
        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null || targetable.IsDead)
                continue;

            //Priority targets, in order
            if (targetable is Illusion ||
                targetable is MushroomEnemy)
            {
                target = targetable;
            }

            float dist = Vector3.Distance(transform.position, targetable.Transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = targetable;
                Debug.Log($"Target: {targetable}");
            }
        }

        if (target is IllusionistEnemy illusionist && !target.IsDead)
        {
            var illusion = illusionist.OnDetected(out bool targetIllusion);
            if (illusion != null && targetIllusion)
            {
                target = illusion;
            }
        }
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
