using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
class MainTowerDefender 
{
    Transform tower;
    [HideInInspector] Enemy target;
    [HideInInspector] float attackSpeed;
}
public class MainTower : Tower
{
    [SerializeField] List<int> GoldGenerationList;
    List<MainTowerDefender> mianTowerDefender;
    //[SerializeField, Range(1, 10f)]
    //float attackRange = 2f;
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

        currentHP = currentHP == 0 ? maxHP : currentHP;
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
            model.SetActive(true);

            float hpPercent = currentHP / maxHP;
            maxHP = HP[CurrentLevel];
            currentHP = maxHP * hpPercent;
        }
    }

    //bool AcquireTarget()
    //{
    //    Collider[] targets;
    //    targets = Physics.OverlapSphere(transform.position, attackRange, illusionMask);
    //    if (targets.Length == 0)
    //    {
    //        targets = Physics.OverlapSphere(transform.position, attackRange, enemyMask);
    //    }
    //    if (targets.Length > 0)
    //    {
    //        int ClosestTargetIndex = 0;
    //        float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);
    //        for (int i = 1; i < targets.Length; i++)
    //        {
    //            if (MinDist <= MinDist + i)
    //            {
    //                float dist = Vector3.Distance(transform.position, targets[i].transform.position);
    //                if (dist < MinDist)
    //                {
    //                    MinDist = dist;
    //                    ClosestTargetIndex = i;
    //                }
    //            }

    //        }
    //        target = targets[ClosestTargetIndex].GetComponent<Enemy>();
    //        if (target != null)
    //        {
    //            return true;
    //        }
    //        else
    //            return false;
    //    }
    //    target = null;
    //    return false;
    //}
}
