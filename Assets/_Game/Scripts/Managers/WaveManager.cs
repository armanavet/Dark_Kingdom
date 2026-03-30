using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
public class WaveManager : MonoBehaviour, ISaveable
{
    [Header("Set Wave Parameters")]
    [SerializeField] Units units;
    [SerializeField] Wave[] waves;
    [SerializeField] float delayBetweenSpawns;
    List<Enemy> spawnedEnemies = new List<Enemy>();
    Wave enemiesToSpawn;

    int currentWaveIndex = 0;
    bool lastEnemy = false;
    bool cantFindPath;
    bool isLastWave;
    [Header("for testing")]
    public int waveLength;

    public int CurrentWaveIndex => currentWaveIndex;
    public int CurrentWave => currentWaveIndex + 1;
    #region Singleton 
    private static WaveManager _instance;
    public static WaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<WaveManager>();
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
    }

    #region Prepare The Spawn
    void TotalEnemiesInWave(int currentWave)
    {
        spawnedEnemies.Clear();
        enemiesToSpawn = waves[currentWave];
    }
    public void GetPhaseCommands()
    {
        if (waves == null || waves.Length == 0) return;

        if (currentWaveIndex == waves.Length - 1) isLastWave = true;
        TotalEnemiesInWave(currentWaveIndex);
        PortalManager.Instance.CalculateActivePortals(currentWaveIndex);
    }
    public void StartSpawn()
    {
        foreach (var portal in PortalManager.Instance.ActivePortals) portal.SpawnTile.Corrupt();
        StartCoroutine(SpawnFlow(PortalManager.Instance.ActivePortals, enemiesToSpawn));
    }
    #endregion
    #region Spawn
    IEnumerator SpawnFlow(List<Portal> activePortals, Wave wave)
    {
        yield return StartCoroutine(ManagePortalVisuals(activePortals, "activate"));

        yield return StartCoroutine(SpawnUnits(activePortals, wave));

        yield return StartCoroutine(ManagePortalVisuals(activePortals, "deactivate"));
    }
    IEnumerator ManagePortalVisuals(List<Portal> activePortals, string action)
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
        int totalEnemies = wave.Enemies.Sum(e => e.Count);
        int spawned = 0;
        int startIndex = Random.Range(0, activePortals.Count);
        foreach (var enemyType in wave.Enemies)
        {
            int count = enemyType.Count;
            GameObject prefab = units.GetByType(enemyType.Type);

            for (int i = 0; i < count; i++)
            {
                if (startIndex > activePortals.Count - 1) startIndex = 0;
                Portal portal = PortalManager.Instance.GetAvailablePortal(startIndex);
                if (portal == null) yield break;

                GameObject enemy = Instantiate
                    (prefab,
                        portal.SpawnTile.transform.position,
                            portal.SpawnTile.pathDirection.GetRotation());
                Enemy script = enemy.GetComponent<Enemy>();
                spawnedEnemies.Add(script);
                script.OnSpawn(portal.SpawnTile, 0f);

                PortalManager.Instance.DestroyPath(portal);

                startIndex++;
                spawned++;
                if (spawned < totalEnemies) yield return new WaitForSeconds(delayBetweenSpawns);
                else lastEnemy = true;
            }
        }
        yield break;
    }
    #endregion
    #region Check The Wave Condition
    public bool IsLastWave()
    {
        return isLastWave;
    }
    void EndWave(bool endImmediately = false)
    {
        currentWaveIndex++;
        enemiesToSpawn = null;
        spawnedEnemies.Clear();
        PortalManager.Instance.Clear();
        GameState nextState;
        if (endImmediately == true)
            nextState = GameState.End;
        else
            nextState = isLastWave ? GameState.End : GameState.Passive;

        StateManager.Instance.ChangeGameStateTo(nextState);
    }
    void RepeatLastWave()
    {
        TotalEnemiesInWave(currentWaveIndex);
        StartCoroutine(SpawnUnits(PortalManager.Instance.ActivePortals, enemiesToSpawn));
    }
    void Check()
    {
        if (IsLastWave() && PortalManager.Instance.HasActivePortal())
        {
            RepeatLastWave();
            return;
        }
        EndWave();
    }
    public void OnEnemyDeath(Enemy enemy)
    {
        if (!spawnedEnemies.Contains(enemy)) return;

        spawnedEnemies.Remove(enemy);
        if (spawnedEnemies.Count <= 0 && lastEnemy == true)
            Check();
    }
    #endregion
    #region Link with Portal Manager
    private void OnEnable()
    {
        StateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        GameManager.Instance.OnWaveIntroFinished += StartSpawn;
        PortalManager.Instance.OnAllPortalsDestroyed += HandleAllPortalsDestroyed;
    }
    private void OnDisable()
    {
        if (PortalManager.Instance != null)
            PortalManager.Instance.OnAllPortalsDestroyed -= HandleAllPortalsDestroyed;

        if (GameManager.Instance != null)
            GameManager.Instance.OnWaveIntroFinished -= StartSpawn;

        if (StateManager.Instance != null)
            StateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }
    void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Passive:
                GetPhaseCommands();
                break;

            case GameState.Active:
                break;
        }
    }
    private void HandleAllPortalsDestroyed()
    {
        EndWave(endImmediately: true);
    }
    #endregion

    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    public string GetUniqueSaveID()
    {
        return nameof(WaveManager);
    }

    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.CurrentWave = currentWaveIndex;
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        currentWaveIndex = saveData.CurrentWave;
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