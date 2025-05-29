using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Units units;
    [SerializeField] Wave[] waves;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] GameObject enemyPathPrefab;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] int spawnerDestroyTime;
    [SerializeField] int spawnDistanceFromCenter;
    [SerializeField, FloatRangeSliderAttribute(-10f, 10f)] FloatRange distanceVariance = new FloatRange(0f);
    bool cantFindPath => spawnPoint.NextOnPath == null;
    List<GameObject> enemyPath = new List<GameObject>();
    Tile spawnPoint;
    int enemyCount;

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
    }

    public void SpawnWave(int wave)
    {
        if (waves == null || waves.Length == 0) return;
        if (wave >= waves.Length) return;

        Wave enemiesToSpawn = waves[wave];
        GameObject spawner = Instantiate(spawnerPrefab, spawnPoint.transform.localPosition, spawnPoint.pathDirection.GetRotation());
        StartCoroutine(SpawnUnits(enemiesToSpawn, spawner));
    }

    void CalculateSpawnPoint()
    {
        List<Tile> potentialPoints = new List<Tile>();
        int variance = (int)distanceVariance.RandomValueInRange;

        foreach (var tile in GameBoard.Instance.tiles)
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
                enemy.GetComponent<Enemy>().OnSpawn(spawnPoint, 0f);
                enemyCount++;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        Destroy(spawner, spawnerDestroyTime);
    }

    public void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount <= 0)
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
}

#region Wave
public enum UnitType
{
    hayvan
}

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
