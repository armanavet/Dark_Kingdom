using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainTowerDefender
{
    public Transform turret;
    public Transform turretRoot;
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
    [Header("Main Tower Parameters")]
    [SerializeField] List<int> GoldGenerationList;
    [SerializeField] private List<MainTowerDefender> Defender;
    [SerializeField] List<float> ShootingPointPositions;
    [SerializeField] List<Transform> TowersRootPoints;
    [SerializeField] private TowersRootPointPositions[] TowersRootPointPositions;
    [SerializeField] float ProjectileSpeed;
    [SerializeField] float AttackSpeed;
    [SerializeField, Range(1, 10f)]
    float attackRange = 2f;

    public Vector3 size = Vector3.one;
    [Range(0f, 1f)]
    public float alpha = 0.5f;

    float damage;
    Vector3 range;
    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        CrystelGenerated = GoldGenerationList[CurrentLevel];
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
        }
        range = new Vector3(attackRange, attackRange, attackRange);
        //TowerSoundData = AudioManager.Instance.SetData(TowerSoundData, SoundDataType.Tower, MixerType.Tower,towerType);
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
                defender.cooldown = 1 / AttackSpeed;
            }
            defender.cooldown -= Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //foreach (var defender in Defender)
        //{
        //    position = defender.turretRoot.position;
        //    Gizmos.DrawCube(position, attackRange);
        //}
        ////Vector3 posiion = transform.position;
        //Gizmos.color = Color.red;
        ////if (target != null)
        ////{
        ////    Gizmos.DrawLine(transform.position, target.transform.position);
        ////}
        ///


        // Set the color with custom alpha.
        Gizmos.color = new Color(0f, 1f, 0f, alpha); // Green with custom alpha
        Vector3 position = new Vector3();

        // Draw the cube.
        foreach (var defender in Defender)
        {
            position = defender.turretRoot.position;
            Gizmos.DrawCube(position, size);
        }
        // Draw a wire cube outline.
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireCube(position, size);
    }
    public override void Upgrade()
    {
        if (CurrentLevel < UpgradePrices.Count)
        {
            //StartCoroutine(PlayUpdateSfx());
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeCrystelAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];
            damage = Damage[CurrentLevel];
            CrystelGenerated = GoldGenerationList[CurrentLevel];

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
        //StopCoroutine(PlayUpdateSfx());

    }

    void Shoot(MainTowerDefender defender)
    {
        //AudioManager.Instance.Play(TowerSoundData,defender.turret.transform, ClipType.OnLaunch_Tower, towerType);
        Vector3 point = defender.target.transform.position;
        float travelDistance = Vector3.Distance(defender.turret.position, point);
        float travelTime = travelDistance / ProjectileSpeed;
        GameObject newProjectile = Instantiate(projectile, defender.turret.position, Quaternion.LookRotation(point - defender.turret.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(ProjectileSpeed);
        StartCoroutine(HitTarget(defender, newProjectile, travelTime));
    }

    bool AcquireTarget(MainTowerDefender defender)
    {

        Collider[] PotentialTargets = Physics.OverlapBox(defender.turretRoot.position, range, Quaternion.identity, illusionMask);
        if (PotentialTargets.Length == 0) PotentialTargets = Physics.OverlapBox(defender.turretRoot.position, range, Quaternion.identity, enemyMask);
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
            towersRootPoints[j].position = new Vector3(towersRootPointPositions[currentLevel].x[j], towersRootPoints[j].position.y * 0, towersRootPointPositions[currentLevel].y[j]);
        }
    }
    IEnumerator HitTarget(MainTowerDefender defender, GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        if (defender.target != null)
        {
            UIManager.Instance.ShowDamage(hitPointPopup, defender.target, damage);
            defender.target.ApplyDamage(damage);

            //foreach (var debuff in currentDebuffs)
            //{
            //    DebuffManager.Instance.ApplyDebuff(defender.target, debuff);
            //}
        }
        //AudioManager.Instance.Play(TowerSoundData, currentProjectile.transform, ClipType.OnHit_Tower,towerType);
        Destroy(currentProjectile);
    }
    public override IEnumerator PlayUpdateSfx()
    {
        //AudioManager.Instance.Play(TowerSoundData, transform, ClipType.OnUpgradeVisualEffect_Tower, towerType);
        //effect = Instantiate(effects[0], new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        //effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Destroy(effect, 2f);
        yield break;
    }
}
