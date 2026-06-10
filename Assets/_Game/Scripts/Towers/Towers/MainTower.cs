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
    [SerializeField] private List<MainTowerDefender> Defenders;
    [SerializeField] private TowersRootPointPositions[] TowersRootPointPositions;
    [SerializeField] private List<float> ShootingPointPositions;
    [SerializeField] private List<Transform> TowersRootPoints;
    [SerializeField] private float ProjectileSpeed;
    private bool canAttack = true;

    protected override void Start()
    {
        soundData = AudioManager.Instance.SetData(Type, SoundDataType.Gameplay);
        worldCanvas.worldCamera = Camera.main;
        TowerPanel.gameObject.SetActive(false);
        TowerPanel.Tower = this;
        currentHP = maxHP;
        UpdateData();
    }

    private void Update()
    {
        CheckStrategy();
        foreach (var defender in Defenders)
        {
            defender.cooldown -= Time.deltaTime;
            AcquireTarget(defender);
            if (defender.target == null || defender.target.IsDead) continue;

            if (canAttack && defender.cooldown <= 0)
            {
                Shoot(defender);
                defender.cooldown = cooldown;
            }
        }
    }

    protected override void UpdateData()
    {
        base.UpdateData();

        UIManager.Instance.MainTowerHB.SetMaxHealth(maxHP);
        UIManager.Instance.MainTowerHB.SetHealth(currentHP);

        ApplyTowerPositionsForLevel(currentLevel, TowersRootPoints, TowersRootPointPositions);
        foreach (var defender in Defenders)
        {
            defender.turret.position = new Vector3(defender.turret.position.x, (defender.turret.position.y * 0) + ShootingPointPositions[currentLevel], defender.turret.position.z);
            defender.cooldown = cooldown;
        }
    }

    void Shoot(MainTowerDefender defender)
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.TowerShoot, soundData, defender.turret.transform);
        Vector3 point = defender.target.Transform.position;
        float travelDistance = Vector3.Distance(defender.turret.position, point);
        float travelTime = travelDistance / ProjectileSpeed;
        GameObject newProjectile = Instantiate(projectile, defender.turret.position, Quaternion.LookRotation(point - defender.turret.position));
        newProjectile.GetComponent<Arrow>()?.Initialize(ProjectileSpeed);
        StartCoroutine(HitTarget(defender, newProjectile, travelTime));
    }

    private void AcquireTarget(MainTowerDefender defender)
    {
        if (defender.target != null && !defender.target.IsDead) return;

        float minDist = float.MaxValue;
        Collider[] hits = Physics.OverlapSphere(defender.turretRoot.position, range, TargetMask);
        foreach (var hit in hits)
        {
            var targetable = hit.GetComponent<ITargetable>();
            if (targetable == null || targetable.IsDead)
                continue;

            //Priority targets, in order
            if (targetable is Illusion ||
                targetable is MushroomEnemy)
            {
                defender.target = targetable;
            }

            float dist = Vector3.Distance(transform.position, targetable.Transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                defender.target = targetable;
                Debug.Log($"Target: {targetable}");
            }
        }

    }

    void ApplyTowerPositionsForLevel(
        int currentLevel,
        List<Transform> towersRootPoints,
        TowersRootPointPositions[] towersRootPointPositions)
    {
        if (currentLevel < 1 || currentLevel >= towersRootPointPositions.Length)
            return;

        var levelData = towersRootPointPositions[currentLevel - 1];

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

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, soundData, currentProjectile.transform);
        if (defender.target != null)
        {
            if (defender.target is Enemy enemyTarget)
                UIManager.Instance.ShowDamage(enemyTarget, damage);
            defender.target.ApplyDamage(damage);
        }
        Destroy(currentProjectile);
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
