using UnityEngine;
using System.Collections.Generic;
using AudioSystem;


public class MushroomEnemy : Enemy
{
    // How it works:
    //Operates within a radius of one tile.
    // Behavior:
    //When it detects one or more towers, it stops moving and begins the capture process.
    //Once a tower is captured, it converts it to its side (making it hostile to other towers).
    //If there are no towers nearby, it destroys the captured ones and continues moving until it finds new targets.
    //After it is destroyed, all captured towers are returned to the player.
    // Captured tower behavior:
    //Captured towers attack normal (player-owned) towers.
    // Normal tower behavior:
    //Captured towers become the highest-priority targets for normal towers. 
    [SerializeField] LayerMask capturedTargetMask;
    [SerializeField] LayerMask targetDefaultMask;
    [SerializeField, Range(1, 3f)]
    float CaptureRange = 2f;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float duration;

    private HashSet<Tower> capturedTargets = new();
    private static readonly int IsCapturing = Animator.StringToHash("isCapturing");

    Vector3 start, end, pos;

    private void Start()
    {
        SetParameters();
        IsAttacking = IsCapturing;
    }

    private void OnDestroy()
    {
        foreach (var tower in capturedTargets)
        {
            if (tower != null)
                tower.OnDestroyed -= OnTowerDestroyed;
        }

        capturedTargets.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 posiion = transform.position;
        Gizmos.DrawWireSphere(posiion, CaptureRange);

    }
    private void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;
        if (state == EnemyState.Dead) return;

        if (AcquireTargets()) StartCapturing();
        else if (CanMove()) Move();
        else Taunting();
    }
    void Taunting()
    {
        animator.SetBool(IsMoving, false);
        animator?.SetBool(IsCapturing, true);
    }
    bool CanMove()
    {
        if (tileTo.DistanceToDestinationOriginal == 0) return false;
        if (capturedTargets.Count == 0) return true;
        return false;
    }
    protected override bool AcquireTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, CaptureRange, towerMask);

        if (targets.Length > 0)
        {
            bool foundAny = false;
            foreach (var target in targets)
            {
                Tower tower = target.GetComponentInChildren<Tower>();
                if (tower == null) continue;
                if (tower.Type == TowerType.MainTower || tower.Type == TowerType.CrystalMine) continue;
                if (capturedTargets.Add(tower))
                {
                    //tower.SetFaction(Faction.Enemy);
                    tower.OnDestroyed += OnTowerDestroyed;
                    foundAny = true;
                }
            }
            return foundAny || capturedTargets.Count > 0;
        }
        return false;
    }
    void OnTowerDestroyed(Tower target)
    {
        if (capturedTargets.Remove(target))
        {
            target.OnDestroyed -= OnTowerDestroyed; ;
        }
    }
    protected void StartCapturing()
    {
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsCapturing, true);
    }
    void ReleaseCapturedTargets()
    {
        if (capturedTargets != null)
        {
            foreach (var target in capturedTargets)
            {
                //target.SetFaction(Faction.Player);
                target.OnDestroyed -= OnTowerDestroyed;
            }
            capturedTargets.Clear();
        }
    }
    protected override void OnDeath()
    {
        state = EnemyState.Dead;

        if (animator != null)
        {

            animator?.SetBool("isDead", true);
        }

        ReleaseCapturedTargets();
        SoundData = new SoundData();
        WaveManager.Instance.OnEnemyDeath(this);
        gameObject.layer = 0;
    }
    protected override void Attack() { }

}
