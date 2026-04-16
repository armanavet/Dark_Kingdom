using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainTowerDefender
{
    public Transform turret;
    public Transform turretRoot;
    [HideInInspector] public ITargetable target;
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
    public override int GetTargetPriority() => 50;

    private void Start()
    {
        EconomyManager.Instance.OnEconomicStructureChange(this);
        CrystalGenerated = GoldGenerationList[CurrentLevel];
        SellPrice = SellPrices[CurrentLevel];
        UpgradePrice = UpgradePrices[CurrentLevel];
        damage = Damage[CurrentLevel];
        maxHP = HP[CurrentLevel];
        model = Models[CurrentLevel];
        projectile = Projectiles[CurrentLevel];
        currentHP = currentHP == 0 ? maxHP : currentHP;
        UIManager.Instance.MainTowerHB.SetMaxHealth(maxHP);
        UIManager.Instance.MainTowerHB.SetHealth(currentHP);
        ApplyTowerPositionsForLevel(CurrentLevel, TowersRootPoints, TowersRootPointPositions);
        UpdateCanvasHeight(CurrentLevel);
        foreach (var defender in Defender)
        {
            defender.turret.position = new Vector3(defender.turret.position.x, (defender.turret.position.y * 0) + ShootingPointPositions[CurrentLevel], defender.turret.position.z);
            defender.cooldown = 1 / AttackSpeed;
        }
        range = new Vector3(attackRange, attackRange, attackRange);
        SetSoundData();
    }

    private void Update()
    {
        foreach (var defender in Defender)
        {
            defender.cooldown -= Time.deltaTime;
            if (StrategyManager.Instance.CurrentStrategy != StrategyType.Battle) continue;

            if (defender.cooldown <= 0)
            {
                if (AcquireTarget(defender))
                {
                    Shoot(defender);
                }
                defender.cooldown = 1 / AttackSpeed;
            }
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
            OnUpgrade();
            UpgradePrice = UpgradePrices[CurrentLevel];
            EconomyManager.Instance.ChangeCrystelAmount(-UpgradePrice);

            CurrentLevel++;
            if (CurrentLevel < UpgradePrices.Count)
                UpgradePrice = UpgradePrices[CurrentLevel];
            SellPrice = SellPrices[CurrentLevel];
            damage = Damage[CurrentLevel];
            CrystalGenerated = GoldGenerationList[CurrentLevel];

            model.SetActive(false);
            model = Models[CurrentLevel];
            ApplyTowerPositionsForLevel(CurrentLevel, TowersRootPoints, TowersRootPointPositions);
            foreach (var defender in Defender)
            {
                defender.turret.position = new Vector3(defender.turret.position.x, (defender.turret.position.y * 0) + ShootingPointPositions[CurrentLevel], defender.turret.position.z);
            }
            model.SetActive(true);

            UpdateCanvasHeight(CurrentLevel);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
            UIManager.Instance.MainTowerHB.SetMaxHealth(maxHP);
            UIManager.Instance.MainTowerHB.SetHealth(currentHP);
        }

    }

    void Shoot(MainTowerDefender defender)
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, SoundData, defender.turret.transform);
        Vector3 point = defender.target.GetTransform().position;
        float travelDistance = Vector3.Distance(defender.turret.position, point);
        float travelTime = travelDistance / ProjectileSpeed;
        GameObject newProjectile = Instantiate(projectile, defender.turret.position, Quaternion.LookRotation(point - defender.turret.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(ProjectileSpeed);
        StartCoroutine(HitTarget(defender, newProjectile, travelTime));
    }

    bool AcquireTarget(MainTowerDefender defender)
    {
        //Collider[] PotentialTargets = Physics.OverlapBox(defender.turretRoot.position, range, Quaternion.identity, illusionMask);
        //if (PotentialTargets.Length == 0) PotentialTargets = Physics.OverlapBox(defender.turretRoot.position, range, Quaternion.identity, enemyMask);
        //if (PotentialTargets.Length > 0)
        //{
        //    if (defender.target == null)
        //    {
        //        Collider closestTarget = PotentialTargets[0];
        //        float minDistance = Vector3.Distance(defender.turret.position, closestTarget.transform.position);
        //        foreach (var potentialTargets in PotentialTargets)
        //        {
        //            float distance = Vector3.Distance(defender.turret.position, potentialTargets.transform.position);
        //            if (distance < minDistance)
        //            {
        //                minDistance = distance;
        //                closestTarget = potentialTargets;
        //            }
        //        }
        //        if (closestTarget != null)
        //        {
        //            defender.target = closestTarget.GetComponent<Enemy>();
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
        //defender.target = null;
        //return false;

        Collider[] hits = Physics.OverlapSphere(defender.turretRoot.position, attackRange, hittabelMask);

        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            Debug.Log($"1 Target: {targetable}, {hit.gameObject}");
            if (targetable == null)
                continue;
            Debug.Log($"2 Target is not null: {targetable}");

            if (targetable.GetFaction() == this.GetFaction())
                continue;
            Debug.Log($"3 Target is not from the same faction: {targetable.GetFaction()}");

            float dist = Vector3.Distance(transform.position, targetable.GetTransform().position);
            float priority = targetable.GetTargetPriority();
            float healthPrecent = targetable.GetHealthPrecent();

            float score = priority * 1000f - dist * 2f - (healthPrecent * 200f);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = targetable;
                Debug.Log($"Target: {targetable}, Score: {score}");
            }
        }

        defender.target = bestTarget;
        return defender.target != null;
    }
    void ApplyTowerPositionsForLevel(
        int currentLevel,
        List<Transform> towersRootPoints,
        TowersRootPointPositions[] towersRootPointPositions)
    {
        if (currentLevel < 0 || currentLevel >= towersRootPointPositions.Length)
            return;

        var levelData = towersRootPointPositions[currentLevel];

        for (int i = 0; i < towersRootPoints.Count; i++)
        {
            if (i >= levelData.x.Length || i >= levelData.y.Length)
                continue;

            var point = towersRootPoints[i];
            var pos = point.localPosition;

            point.localPosition = new Vector3(levelData.x[i], pos.y, levelData.y[i]);
        }
    }
    IEnumerator HitTarget(MainTowerDefender defender, GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, SoundData, currentProjectile.transform);
        if (defender.target != null)
        {
            if (defender.target is Enemy enemyTarget)
                UIManager.Instance.ShowDamage(unitHitPointPopup, enemyTarget, damage);
            defender.target.ApplyDamage(damage);
        }
        Destroy(currentProjectile);
    }
}
