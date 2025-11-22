using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.ComponentModel.Design;
using Unity.VisualScripting;

public class WaveManager : MonoBehaviour, ISaveable
{
    [Header("Set Wave Parameters")]       
        [SerializeField] Units units;
        [SerializeField] Wave[] waves;
        [SerializeField] GameObject spawnerPrefab;
        [SerializeField] GameObject enemyPathPrefab;
        [SerializeField] float delayBetweenSpawns;
        [SerializeField] float spawnerDestroyTime;
        [SerializeField] int spawnDistanceFromCenter;
        [SerializeField, FloatRangeSlider(-10f, 10f)] FloatRange distanceVariance = new FloatRange(0f);
    [Header("For Fast testing")]
        [SerializeField] bool a;
        [SerializeField] Tile SpawnPoint;
    
    GameObject spawner;
    List<GameObject> enemyPath = new List<GameObject>();
    List<Enemy> enemies = new List<Enemy>();
    Wave enemiesToSpawn;
    //Wave[] reserveWave;//this line is not necessary
    Tile spawnPoint;

    int currentWave;
    bool cantFindPath => spawnPoint.NextOnPath == null;
    bool isPortalDead;

    public PortalMode portalMode;
    public int totalEnemiesInWave = 0;
    public int waveLength;
    public bool isPortalOn;
    public bool isPortalOff;
    #region Singleton 
    private static WaveManager _instance;
    public static WaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WaveManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        RegisterSaveable();
    }
    #endregion
    public void Initialize()
    {
        units.OrganizeByType();
        waveLength = waves.Length;
        isPortalDead = false;
        isPortalOn = false;
        isPortalOff = false;
    }
    public void GetPhaseCommands(int wave, bool isLastWave = false)
    {
        if (waves == null || waves.Length == 0) return;
        if (wave >= waves.Length) return;
        //reserveWave = waves;//this line is not necessary
        currentWave = wave;
        TotalEnemiesInWave(wave);
        DrawEnemyPath();
        if (!isLastWave) portalMode = PortalMode.NormalMode;
        else portalMode = PortalMode.BossMode;
    }
    public void StartSpawn()
    {
        spawner = Instantiate(spawnerPrefab, spawnPoint.transform.localPosition, spawnPoint.pathDirection.GetRotation());
        spawnPoint.Corrupt();
        StartCoroutine(SpawnFlow(spawner, enemiesToSpawn));
    }
    //void StartCoroutines(GameObject spawner, Wave enemies)
    //{
    //    StartCoroutine(SpawnUnits(spawner, enemies));
    //}
    void CalculateSpawnPoint()
    {
        if (a)
        {
            spawnPoint = SpawnPoint;
        }
        else
        {
            List<Tile> potentialPoints = new List<Tile>();
            int variance = (int)distanceVariance.RandomValueInRange;
            foreach (var tile in GameBoard.Instance.Tiles)
            {
                if (tile.DistanceToDestinationOriginal == spawnDistanceFromCenter + variance)
                {
                    potentialPoints.Add(tile);
                }
            }
            spawnPoint = potentialPoints[Random.Range(0, potentialPoints.Count - 1)];
        }
    }
    public void TotalEnemiesInWave(int currentWave)
    {
        enemiesToSpawn = waves[currentWave];
        totalEnemiesInWave = waves[currentWave].Enemies.Sum(e => e.Count);
    }
    IEnumerator SpawnFlow(GameObject spawner, Wave wave)
    {
        // 1. Pre-activate portal
        yield return Portal.Instance.ActivateVisual();
        // 2. Spawn enemies
        yield return new WaitUntil(() => isPortalOn == true);
        yield return SpawnUnits(spawner, wave);
        // 3. Deactivate portal
        yield return new WaitUntil(() => isPortalOn == false);
        yield return Portal.Instance.DeactivateVisual();
        // 4. Destroy portal
        yield return new WaitUntil(() => isPortalOff == true);
        Destroy(spawner, spawnerDestroyTime);
        yield break;
    }

    IEnumerator SpawnUnits(GameObject spawner, Wave wave)
    {
        foreach (var enemyType in wave.Enemies)
        {
            int count = enemyType.Count;
            GameObject prefab = units.GetByType(enemyType.Type);
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.pathDirection.GetRotation());
                Enemy script = enemy.GetComponent<Enemy>();
                enemies.Add(script);
                script.OnSpawn(spawnPoint, 0f);
                foreach (var path in enemyPath)
                {
                    Destroy(path);
                }
                enemyPath.Clear();

                bool islastUint = (totalEnemiesInWave == 1);
                if (!islastUint)
                    yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        yield break;
    }

    void IsWaveEnd()
    {
        if (portalMode == PortalMode.NormalMode)
        {
            if (totalEnemiesInWave == 0)
            {
                isPortalOn = false;
                enemiesToSpawn = null;
                Debug.Log("2 "+isPortalOn);
                enemies.Clear();
                spawnPoint.Restore();
                enemyPath.ForEach(x => Destroy(x));
                StateManager.Instance.ChangeGameStateTo(GameState.Passive);
            }
        }
        else if (portalMode == PortalMode.BossMode)
        {
            if (isPortalDead == true)
            {
                isPortalOn = false;
                enemiesToSpawn = null;
                enemies.Clear();
                spawnPoint.Restore();
                enemyPath.ForEach(x => Destroy(x));
                StateManager.Instance.ChangeGameStateTo(GameState.End);
            }
            else if (isPortalDead == false && totalEnemiesInWave == 0)
            {
                TotalEnemiesInWave(currentWave);
                StartCoroutine(SpawnUnits(spawner, enemiesToSpawn));
            }
        }
    }
    public void OnEnemyDeath(Enemy enemy)
    {
        if (portalMode == PortalMode.NormalMode && !enemies.Contains(enemy))
        {
            return;
        }
        else if (portalMode == PortalMode.BossMode && !enemies.Contains(enemy))
        {
            isPortalDead = true;
        }
        enemies.Remove(enemy);
        totalEnemiesInWave--;
        Debug.Log("1 "+isPortalOn);
        IsWaveEnd();

    }
    /*public void OnPortalDeath(Enemy enemy)
    {
        isPortalDead = true;
        isPortalDeath == true
        enemypath => destroy()
        enemies remove all
        change state to end
         
    }*/
    void DrawEnemyPath()
    {
        CalculateSpawnPoint();
        Tile tile = spawnPoint;
        GameBoard.Instance.BuildPathToDestination(ignoreTowers: false);
        if (cantFindPath) GameBoard.Instance.BuildPathToDestination(ignoreTowers: true);

        while (tile != null && tile.Type != TileType.Destination)
        {
            GameObject path = Instantiate(enemyPathPrefab, tile.transform.position, tile.pathDirection.GetRotation());
            enemyPath.Add(path);
            tile = tile.NextOnPath;
        }
    }
    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);
    public string GetUniqueSaveID()
    {
        return nameof(WaveManager);
    }
    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.EnemySpawnTile = spawnPoint.Index;
        return saveData;
    }
    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        spawnPoint = GameBoard.Instance.Tiles[saveData.EnemySpawnTile];
    }
}
#region Wave
[System.Serializable]
public class Unit
{
    public GameObject Prefab;
    public UnitType Type;
}

[System.Serializable]
public class Units
{
    public Unit[] UnitList;
    Dictionary<UnitType, GameObject> UnitsByType = new Dictionary<UnitType, GameObject>();

    public void OrganizeByType()
    {
        foreach (var unit in UnitList)
        {
            UnitsByType.Add(unit.Type, unit.Prefab);
        }
    }

    public GameObject GetByType(UnitType type)
    {
        return UnitsByType[type];
    }
}

[System.Serializable]
public class Wave
{
    public EnemiesToSpawn[] Enemies;
}

[System.Serializable]
public class EnemiesToSpawn
{
    public UnitType Type;
    public int Count;
}

#endregion

#region PortalPhase
public enum PortalMode
{
    NormalMode,
    BossMode
}
#endregion

