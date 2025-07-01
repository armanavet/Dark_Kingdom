using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour, ISaveable
{
    [SerializeField] Units units;
    [SerializeField] Wave[] waves;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] GameObject enemyPathPrefab;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] int spawnerDestroyTime;
    [SerializeField] int spawnDistanceFromCenter;
    [SerializeField, FloatRangeSlider(-10f, 10f)] FloatRange distanceVariance = new FloatRange(0f);
    bool cantFindPath => spawnPoint.NextOnPath == null;
    List<GameObject> enemyPath = new List<GameObject>();
    List<Enemy> enemies = new List<Enemy>();
    Tile spawnPoint;

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
    }
    #endregion

    void Start()
    {
        units.OrganizeByType();
        SaveManager.RegisterSaveable(this);
    }

    public void SpawnWave(int wave)
    {
        if (waves == null || waves.Length == 0) return;
        if (wave >= waves.Length) return;

        Wave enemiesToSpawn = waves[wave];
        spawnPoint.Corrupt();
        GameObject spawner = Instantiate(spawnerPrefab, spawnPoint.transform.localPosition, spawnPoint.pathDirection.GetRotation());
        StartCoroutine(SpawnUnits(enemiesToSpawn, spawner));
    }

    void CalculateSpawnPoint()
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

    IEnumerator SpawnUnits(Wave wave, GameObject spawner)
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
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        Destroy(spawner, spawnerDestroyTime);
        spawnPoint.Restore();
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        if (!enemies.Contains(enemy)) return;

        enemies.Remove(enemy);
        if (enemies.Count == 0)
        {
            enemyPath.ForEach(x => Destroy(x));
            StateManager.Instance.ChangeGameStateTo(GameState.Passive);
        }
    }

    public void DrawEnemyPath()
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
