using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryTower : Tower
{
    [SerializeField, Range(1, 10f)]
    float TarggetPoint = 2f;
    float TarggetRange = 2f;
    float launchSpeed;
    float g = 9.81f;
    Enemy target;
    [SerializeField] Shel shel;
    float launchProgress = 0f;
    int shotsPerSecond = 1;
    [SerializeField] Transform mortal;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1;
    [SerializeField, Range(1, 200)]
    float shellDamage = 30;

    void Awake()
    {
        float x = TarggetRange + 0.250001f;
        float y = -mortal.position.y;
        launchSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
    }

    void Start()
    {
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        shellDamage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        if (Debuffs[CurrentLevel] != null)
            currentDebuffs.Add(Debuffs[CurrentLevel]);

        currentHP = currentHP == 0 ? maxHP : currentHP;
    }

    void Update()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        if (launchProgress > 1)
        {
            if (AcquireTarget())
            {
                Launch(target);
            }
            launchProgress = 0;
        }
    }

    void Launch(Enemy target)
    {
        if (target == null)
        {
            return;
        }
        Vector2 dir;
        Vector3 launchPoint = mortal.position;
        Vector3 TargetPoint = target.transform.position;
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
        
        Shel sh = Instantiate(shel);
        sh.Initialize(launchPoint, TargetPoint, new Vector3(s * CosTheta * dir.x, s * sinTheta, s * CosTheta * dir.y), shellBlastRadius, shellDamage,currentDebuffs);
        
    }
    bool AcquireTarget()
    {
        Collider[] targets;
        targets = Physics.OverlapSphere(transform.position, TarggetPoint, illusionMask);
        if (targets.Length == 0)
        {
            targets = Physics.OverlapSphere(transform.position, TarggetPoint, enemyMask);
        }
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
            target = targets[ClosestTargetIndex].GetComponent<Enemy>();
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
    public override void Upgrade()
    {
        if (CurrentLevel < SellPrices.Count - 1 && CurrentLevel < UpgradePrices.Count)
        {
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);

            CurrentLevel++;

            shellDamage = Damage[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }
    }
}
