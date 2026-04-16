using UnityEngine;
using System.Collections.Generic;
using AudioSystem;
using System.Collections;
using Unity.VisualScripting;

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
    bool isJumping;
    public override int GetTargetPriority() => 100;

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
        if (state == EnemyState.Dead) return;


        if (AcquireTargets())
        {
            //Debug.Log("[2] 1. Targets were found. Start the capturing process.");
            StartCapturing();
        }
        else if (CanMove())
        {
            //Debug.Log("[2] 2. There are no targets. Can move. ");
            Move();
        }
        else
        {
            //Debug.Log("[2] 3. There aren't any new targets, but old ones still alive. I can't move. :" + capturedTargets);
            Taunting();
        }
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
        //Debug.Log($"[2] 4. There are still alive towers. {capturedTargets.Count}");
        return false;
    }
    protected override bool AcquireTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, CaptureRange, towerMask);

        //Debug.Log($"[2] 5. Targets were found {targets.Length}");
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
                    //Debug.Log($"[2] 6. Tower: {tower}");
                    tower.SetPriority(Faction.Enemy);
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
                target.SetPriority(Faction.Player);
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
        enemySoundData = new SoundData();
        WaveManager.Instance.OnEnemyDeath(this);
        gameObject.layer = 0;
    }
    //protected override void PrepareNextMove()
    //{
    //    AcquireTargets();
    //    positionFrom = transform.position;
    //    positionTo = tileFrom.exitPoint;

    //    direction = tileFrom.pathDirection;

    //    directionChange = DirectionChange.None;

    //    // Optional: instantly align rotation OR let jump handle it
    //    //transform.rotation = direction.GetRotation();

    //}
    //IEnumerator JumpRoutine()
    //{
    //    isJumping = true;

    //    animator.SetBool(IsCapturing, false);
    //    animator.SetBool(IsMoving, true);

    //    float time = 0;

    //    start = transform.position;
    //    end = tileTo.transform.position;

    //    while (time < duration)
    //    {
    //        float t = time / duration;
    //        t = Mathf.SmoothStep(0, 1f, t);

    //        Vector3 pos = Vector3.Lerp(start, end, t);
    //        pos.y = jumpCurve.Evaluate(t);

    //        transform.position = pos;

    //        time += Time.deltaTime;
    //        yield return null;
    //    }

    //    transform.position = end;

    //    tileFrom = tileTo;
    //    tileTo = tileFrom.NextOnPath;

    //    PrepareNextMove();
    //    isJumping = false;
    //}
    protected override void Attack() { }

}
