using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WormManager : MonoBehaviour
{
    // How it works:
    // X number of worms per wave.
    // The number of worms increases by N each wave.
    // Target selection rate: 40% for miner towers, 60% for action towers.

    // Behavior:
    // Spawns when the wave starts.
    // They appear under random towers and destroy them immediately.

    [SerializeField] GameObject plane;
    [SerializeField] Enemy unit;
    [SerializeField] int amountPerWave = 3;
    [SerializeField] bool multiply;
    [SerializeField] float increaseBy = 1;
    [Header("Probability")]
    [SerializeField] float miner;
    [SerializeField] float action;
    List<Tower> tergetTowers = new List<Tower>();
    List<Tower> minerTowers = new List<Tower>();
    List<Tower> actionTowers = new List<Tower>();
    float minerWeight => miner;
    float actionWeight => action;
    #region Singleton 
    private static WormManager _instance;
    public static WormManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<WormManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //test();
        }
    }
    void test()
    {
        var point = plane.transform.position;
        Instantiate(unit, point, Quaternion.Euler(new Vector3(point.x, 180f, point.z)), transform);
    }
    void GetAmountInWave(int currentWave)
    {
        if (multiply) amountPerWave = (int)(amountPerWave * increaseBy);
        else amountPerWave = (int)(amountPerWave + increaseBy);
    }
    void AdjustCountsForAvailability(
    ref int minerCount,
    ref int actionCount,
    int minerAvailable,
    int actionAvailable)
    {
        if (minerCount > minerAvailable)
        {
            int deficit = minerCount - minerAvailable;
            minerCount = minerAvailable;
            actionCount += deficit;
        }

        if (actionCount > actionAvailable)
        {
            int deficit = actionCount - actionAvailable;
            actionCount = actionAvailable;
            minerCount += deficit;
        }

        minerCount = Mathf.Min(minerCount, minerAvailable);
        actionCount = Mathf.Min(actionCount, actionAvailable);
    }
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    public void CalculateSpawnPoints()
    {
        GetTergets();

        float totalWeight = minerWeight + actionWeight;
        float minerProbability = minerWeight / totalWeight;
        int minerCount = Mathf.RoundToInt(amountPerWave * minerProbability);
        int actionCount = amountPerWave - minerCount;

        AdjustCountsForAvailability(ref minerCount, ref actionCount, minerTowers.Count, actionTowers.Count);
        Shuffle(minerTowers);
        Shuffle(actionTowers);

        tergetTowers.AddRange(minerTowers.GetRange(0, Mathf.Min(minerCount, minerTowers.Count)));
        tergetTowers.AddRange(actionTowers.GetRange(0, Mathf.Min(actionCount, actionTowers.Count)));

        Shuffle(tergetTowers);


    }
    void GetTergets()
    {
        actionTowers.Clear();
        minerTowers.Clear();
        actionTowers.Clear();

        var towers = TowerManager.Instance.Towers;

        foreach (var tower in towers)
        {
            if (tower.Type == TowerType.MainTower) continue;
            bool isMiner = tower.Type == TowerType.CrystalMine;
            (isMiner ? minerTowers : actionTowers).Add(tower);
        }
    }
    public void Spawn()
    {
        if (tergetTowers == null) return;
        else if (unit == null) return;
        Debug.Log("spawn");
        foreach (var target in tergetTowers)
        {
            Debug.Log(target);
            if (target == null) continue;
            var spawnPoint = target.tile.transform.position;
            Enemy enemy = Instantiate(unit, spawnPoint, Quaternion.Euler(new Vector3(spawnPoint.x, Random.Range(15, 280), spawnPoint.z)), transform);
            Debug.Log(enemy);
            target.ApplyDamage(enemy.Damage);
        }
    }

}
