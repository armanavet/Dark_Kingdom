using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalEnemy : Enemy
{
    private void Start()
    {
        health = maxHP;
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
    protected override void Attack() { }
    protected override void PlayAttackSound() { }
    protected override void PlayMovingSound() { }

}
