using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using AudioSystem;
using UnityEngine.UIElements;
class Pathfinder
{
    public List<Portal> CalculateSpawnPoints(List<Portal> allPortals, int amountOf)
    {
        List<Portal> activePortals = allPortals.OrderBy(x => Random.value).Take(amountOf).ToList();
        return activePortals;
    }
}
public class WaveManager : MonoBehaviour, ISaveable
{
    [Header("Set Wave Parameters")]
    [SerializeField] Units units;
    [SerializeField] Wave[] waves;
    [SerializeField] List<Portal> allPortals;
    [SerializeField] GameObject enemyPathPrefab;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] float spawnerDestroyTime;
    [SerializeField] int spawnDistanceFromCenter;
    [SerializeField, FloatRangeSlider(-10f, 10f)] FloatRange distanceVariance = new FloatRange(0f);
    [Header("For Fast testing")]
    [SerializeField] bool a;

    List<Enemy> enemies = new List<Enemy>();
    Wave enemiesToSpawn;
    List<Portal> activePortals;
    Pathfinder pathfinder;
    Dictionary<int, Portal> portalsByID;
    int numberOfActivePortlas = 1;
    int wavesLast = 3;
    int currentWave;
    bool cantFindPath;
    bool isPortalDead;

    public PortalMode portalMode;
    public int totalEnemiesInWave = 0;
    public int waveLength;
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
        portalsByID = new Dictionary<int, Portal>();
        foreach (var portal in allPortals) portalsByID.Add(portal.ID, portal);
        pathfinder = new Pathfinder();
        units.OrganizeByType();
        waveLength = waves.Length;
        isPortalDead = false;
    }
    public void TotalEnemiesInWave(int currentWave)
    {
        enemiesToSpawn = waves[currentWave];
        totalEnemiesInWave = waves[currentWave].Enemies.Sum(e => e.Count);
        
    }
    void DrawEnemyPath(List<Portal> portalsList)
    {
        foreach (var portal in portalsList)
        {
            GameBoard.Instance.BuildPathToDestination(ignoreTowers: false);
            if (portal.SpawnTile.NextOnPath == null) GameBoard.Instance.BuildPathToDestination(ignoreTowers: true);

            portal.path.Clear();
            Tile tile = portal.SpawnTile;
            while (tile != null && tile.Type != TileType.Destination)
            {
                GameObject path = Instantiate(enemyPathPrefab, tile.transform.position, tile.pathDirection.GetRotation());
                portal.path.Add(path);
                tile = tile.NextOnPath;
            }
        }
    }
    int GetNumberOf(int waves, int numberOf, bool isLastWave)
    {
        if (waves == 0 || isLastWave == true)
        {
            numberOf++;
            wavesLast = 3;
        }
        return numberOf;
    }
    public void GetPhaseCommands(int wave, bool isLastWave = false)
    {
        if (waves == null || waves.Length == 0) return;
        if (wave >= waves.Length) return;

        currentWave = wave;

        TotalEnemiesInWave(wave);

        numberOfActivePortlas = GetNumberOf(wavesLast, numberOfActivePortlas, isLastWave);
        activePortals = pathfinder.CalculateSpawnPoints(allPortals, numberOfActivePortlas);

        DrawEnemyPath(activePortals);

        if (!isLastWave) portalMode = PortalMode.NormalMode;
        else portalMode = PortalMode.BossMode;
    }
    public void StartSpawn()
    {
        foreach (var portal in activePortals) portal.SpawnTile.Corrupt();
        StartCoroutine(SpawnFlow(activePortals, enemiesToSpawn));

    }

    IEnumerator SpawnFlow(List<Portal> activePortals, Wave wave)
    {
        yield return StartCoroutine(OpenPortals(activePortals, "activate"));

        yield return StartCoroutine(SpawnUnits(activePortals, wave));

        yield return StartCoroutine(OpenPortals(activePortals, "deactivate"));
    }
    IEnumerator OpenPortals(List<Portal> activePortals, string action)
    {
        int completed = 0;

        foreach (var portal in activePortals)
        {
            StartCoroutine(OpenPortalRoutine(portal, action, () => completed++));
        }
        yield return new WaitUntil(() => completed == activePortals.Count);
    }
    IEnumerator OpenPortalRoutine(Portal portal, string action, Action onComplete)
    {
        if (action == "activate")
            yield return StartCoroutine(portal.ActivateVisual());
        if (action == "deactivate")
            yield return StartCoroutine(portal.DeactivateVisual());
        onComplete?.Invoke();
    }
    IEnumerator SpawnUnits(List<Portal> activePortals, Wave wave)
    {
        int portalIndex = Random.Range(0, activePortals.Count - 1);
        foreach (var enemyType in wave.Enemies)
        {
            int count = enemyType.Count;
            GameObject prefab = units.GetByType(enemyType.Type);

            for (int i = 0; i < count; i++)
            {
                if (portalIndex > activePortals.Count - 1) portalIndex = 0;
                Portal portal = activePortals[portalIndex];

                GameObject enemy = Instantiate(prefab, portal.SpawnTile.transform.position, portal.SpawnTile.pathDirection.GetRotation());
                Enemy script = enemy.GetComponent<Enemy>();

                enemies.Add(script);
                script.OnSpawn(portal.SpawnTile, 0f);

                foreach (var path in portal.path)
                {
                    Destroy(path);
                }
                portal.path.Clear();
                portalIndex++;
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
                enemiesToSpawn = null;
                enemies.Clear();
                activePortals.Clear();
                wavesLast--;
                StateManager.Instance.ChangeGameStateTo(GameState.Passive);
            }
        }
        else if (portalMode == PortalMode.BossMode)
        {
            if (isPortalDead == true)
            {
                enemiesToSpawn = null;
                enemies.Clear();
                StateManager.Instance.ChangeGameStateTo(GameState.End);
            }
            else if (isPortalDead == false && totalEnemiesInWave == 0)
            {
                TotalEnemiesInWave(currentWave);
                Debug.Log(enemiesToSpawn);
                Debug.Log(totalEnemiesInWave);
                StartCoroutine(SpawnUnits(activePortals, enemiesToSpawn));
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
        IsWaveEnd();

    }
    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);
    public string GetUniqueSaveID()
    {
        return nameof(WaveManager);
    }
    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.activePortalIDs.Clear();
        foreach (var portal in activePortals) saveData.activePortalIDs.Add(portal.ID);
        return saveData;
    }
    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        activePortals.Clear();
        foreach (var id in saveData.activePortalIDs)
        {
            if (portalsByID.TryGetValue(id, out Portal portal)) activePortals.Add(portal);
        }
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
            if (unit != null)
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



//activeSpawnPoints.Clear();
//void CalculateSpawnPoint()
//{
//    if (a)
//    {
//        spawnPoint = SpawnPoint;
//    }
//    else
//    {
//        List<Tile> potentialPoints = new List<Tile>();
//        int variance = (int)distanceVariance.RandomValueInRange;
//        foreach (var tile in GameBoard.Instance.Tiles)
//        {
//            if (tile.DistanceToDestinationOriginal == spawnDistanceFromCenter + variance)
//            {
//                potentialPoints.Add(tile);
//            }
//        }
//        spawnPoint = potentialPoints[Random.Range(0, potentialPoints.Count - 1)];
//    }
//}

//List<PortalSpawnPoint> available = new List<PortalSpawnPoint>(allPortals);
//int count = Mathf.Min(activePortals, available.Count);
//var shuffled = allPortals.OrderBy(_ => Random.value).ToList();

//for (int i = 0; i < count; i++)
//{
//    activeSpawnPoints.Add(shuffled[i]);
//}
//activeSpawnPoints = portalManager.GetActivePortals();

//CalculateSpawnPoint();

//Tile tile = spawnPoint;
//GameBoard.Instance.BuildPathToDestination(ignoreTowers: false);
//if (cantFindPath) GameBoard.Instance.BuildPathToDestination(ignoreTowers: true);

//while (tile != null && tile.Type != TileType.Destination)
//{
//    GameObject path = Instantiate(enemyPathPrefab, tile.transform.position, tile.pathDirection.GetRotation());
//    enemyPath.Add(path);
//    tile = tile.NextOnPath;
//}

//for (int i = 1; i < count; i++)
//{
//    int index = Random.Range(0, available.Count - 1);
//    activeSpawnPoints.Add(available[index]);
//    available.RemoveAt(index);
//}
//Debug.Log(activeSpawnPoints);


//foreach (var spawnTile in activeSpawnPoints)
//{
//    GameBoard.Instance.BuildPathToDestination(ignoreTowers: false);
//    if (spawnTile.SpawnTile.NextOnPath == null)
//        GameBoard.Instance.BuildPathToDestination(ignoreTowers: true);

//    Tile tile = spawnTile.SpawnTile;

//    while (tile != null && tile.Type != TileType.Destination)
//    {
//        GameObject path = Instantiate(
//            enemyPathPrefab,
//            tile.transform.position,
//            tile.pathDirection.GetRotation()
//        );

//        enemyPath.Add(path);
//        tile = tile.NextOnPath;
//    }
//}