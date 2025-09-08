using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class MainTowerDefender
{
    public Transform turret;
    [HideInInspector] public Enemy target;
    [HideInInspector] public float cooldown;
}

[System.Serializable]
public class TowersRootPointPositions
{
    public float[] x;
    public float[] y;

}
public class MainTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    [SerializeField] private List<MainTowerDefender> Defender;
    [SerializeField] List<float> ShootingPointPositions;
    [SerializeField] List<Transform> TowersRootPoints;
    [SerializeField] private TowersRootPointPositions[] TowersRootPointPositions;
    [SerializeField] float ProjectileSpeed;
    [SerializeField] float AttackSpeed;
    [SerializeField, Range(1, 10f)]
    float attackRange = 2f;
    float damage;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        GoldGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        projectile = Projectiles[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
        CountTowerPointPositionsOnEachLevel(CurrentLevel, TowersRootPoints, TowersRootPointPositions);
        foreach (var defender in Defender)
        {
            defender.turret.position = new Vector3(defender.turret.position.x, ShootingPointPositions[CurrentLevel], defender.turret.position.z);
            defender.cooldown = 1 / AttackSpeed;
            Debug.Log(defender.cooldown);
        }
    }

    private void Update()
    {
        foreach (var defender in Defender)
        {
            if (defender.cooldown <= 0)
            {
                if (AcquireTarget(defender))
                {
                    Shoot(defender);
                }
                defender.cooldown = Random.Range(0.1f, 1f / AttackSpeed);
            }
            defender.cooldown -= Time.deltaTime;
        }
    }
    public override void Upgrade()
    {
        if (CurrentLevel < UpgradePrices.Count)
        {
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeGoldAmount(-UpgradePrice);

            CurrentLevel++;

            damage = Damage[CurrentLevel];
            GoldGenerated = GoldGenerationList[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            CountTowerPointPositionsOnEachLevel(CurrentLevel, TowersRootPoints, TowersRootPointPositions);
            foreach (var defender in Defender)
            {
                defender.turret.position = new Vector3(defender.turret.position.x, ShootingPointPositions[CurrentLevel], defender.turret.position.z);
            }
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }


    }

    void Shoot(MainTowerDefender defender)
    {
        Vector3 point = defender.target.transform.position;
        float travelDistance = Vector3.Distance(defender.turret.position, point);
        float travelTime = travelDistance / ProjectileSpeed;
        GameObject newProjectile = Instantiate(projectile, defender.turret.position, Quaternion.LookRotation(point - defender.turret.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(ProjectileSpeed);
        StartCoroutine(HitTarget(defender, newProjectile, travelTime));
    }

    bool AcquireTarget(MainTowerDefender defender)
    {
        Collider[] PotentialTargets = Physics.OverlapSphere(transform.position, attackRange, illusionMask);
        if (PotentialTargets.Length == 0) PotentialTargets = Physics.OverlapSphere(transform.position, attackRange, enemyMask);
        if (PotentialTargets.Length > 0)
        {
            if (defender.target == null)
            {
                Collider closestTarget = PotentialTargets[0];
                float minDistance = Vector3.Distance(defender.turret.position, closestTarget.transform.position);
                foreach (var potentialTargets in PotentialTargets)
                {
                    float distance = Vector3.Distance(defender.turret.position, potentialTargets.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = potentialTargets;
                    }
                }
                if (closestTarget != null)
                {
                    defender.target = closestTarget.GetComponent<Enemy>();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        defender.target = null;
        return false;

    }

    void CountTowerPointPositionsOnEachLevel(int currentLevel, List<Transform> towersRootPoints, TowersRootPointPositions[] towersRootPointPositions)
    {
        for (int j = 0; j < towersRootPoints.Count; j++)
        {
            towersRootPoints[j].position = new Vector3(towersRootPointPositions[currentLevel].x[j], towersRootPoints[j].position.y, towersRootPointPositions[currentLevel].y[j]);
        }
    }
    IEnumerator HitTarget(MainTowerDefender defender, GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        if (defender.target != null)
        {
            defender.target.ApplyDamage(damage);
            //foreach (var debuff in currentDebuffs)
            //{
            //    DebuffManager.Instance.ApplyDebuff(defender.target, debuff);
            //}
        }

        Destroy(currentProjectile);
    }
}
