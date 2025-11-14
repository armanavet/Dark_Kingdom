using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalEnemy : Enemy
{
    [SerializeField] string portalLayerName = "Portal";
    private void Start()
    {
        health = maxHP;
        if(WaveManager.Instance.portalMode == PortalMode.BossMode)
        {
            gameObject.layer = LayerMask.NameToLayer(portalLayerName);
        }
    }
    private void Update()
    {
        if (state == EnemyState.Dead) return;
    }
    protected override void OnDeath()
    {
        state = EnemyState.Dead;
        WaveManager.Instance.OnEnemyDeath(this);
        gameObject.layer = 0;
    }
    protected override void Attack(){}
}
