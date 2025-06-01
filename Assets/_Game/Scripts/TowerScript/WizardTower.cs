using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTower : Tower
{
    [SerializeField, Range(1, 10f)]
    float TarggetPoint = 2f;
    float TarggetRange = 2f;
    public LayerMask EnemyMask;
    float launchSpeed;
    float g = 9.81f;
    TargetPoint target;
    [SerializeField] Shel shel;
    float launchProgress = 0f;
    int shotsPerSecond = 1;
    [SerializeField] Transform mortal;
    [SerializeField, Range(0.5f, 5f)]
    float shellBlastRadius = 1;
    [SerializeField, Range(1, 200)]
    float shellDamage = 30;
    private void Awake()
    {
        float x = TarggetRange + 0.250001f;
        float y = -mortal.position.y;
        launchSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
        maxHP = HP[levelOFTower];
        currentHP=maxHP;
    }
    // Update is called once per frame
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
    void Launch(TargetPoint target)
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
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        float CosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);
        Debug.Log("s="+s+ " CosTheta="+ CosTheta+" dir="+dir+ " sinTheta="+ sinTheta +" r=" +r);

        //mortal.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));
        Shel sh = Instantiate(shel);
        sh.Initialize(launchPoint, TargetPoint, new Vector3(s * CosTheta * dir.x, s * sinTheta, s * CosTheta * dir.y), shellBlastRadius, shellDamage);
        //Vector3 prev = launchPoint;
        //Vector3 next = launchPoint;
        //for (int i = 0; i < 10; i++)
        //{
        //    float t = i / 10f;
        //    float dx = s * CosTheta * t;
        //    float dy = s * sinTheta * t - 0.5f * g * t * t;
        //    next=launchPoint+new Vector3(dir.x*dx,dy,dir.y*dx);
        //    Debug.DrawLine(prev,next,Color.blue);
        //    prev=next;
        //}
        //Debug.DrawLine(launchPoint, TargetPoint,Color.yellow);
    }
    bool AcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, TarggetPoint, EnemyMask);
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
            target = targets[ClosestTargetIndex].GetComponent<TargetPoint>();
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
        if (levelOFTower < SellPrices.Count - 1 && levelOFTower < UpgradePrices.Count)
        {
            SellPrice = SellPrices[levelOFTower + 1];
            UpgradePrice = UpgradePrices[levelOFTower];
            Economics.Instance.ChangeGoldAmount(-UpgradePrice);
            levelOFTower++;
        }
    }
}
