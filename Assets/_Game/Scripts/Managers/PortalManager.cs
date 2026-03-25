using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class PortalManager : MonoBehaviour, ISaveable
{
    [SerializeField] Portal portalPrefab;
    [SerializeField] GameObject enemyPathPrefab;
    [SerializeField] List<Tile> SpawnPoints;
    [HideInInspector] public List<Portal> ActivePortals = new();
    List<Portal> allPortals = new();
    int numberOfActivePortals = 1;
    Dictionary<int, Portal> portalsByID;
    GameState currentState;
    public delegate void PortalsDestroyed();
    public event PortalsDestroyed OnAllPortalsDestroyed;
    #region Singleton 
    private static PortalManager _instance;
    public static PortalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PortalManager>();
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
        StorePortals();
    }
    void StorePortals()
    {
        SpawnPortals();
        if (allPortals == null) return;
        portalsByID = new Dictionary<int, Portal>();
        foreach (var portal in allPortals) portalsByID.Add(portal.ID, portal);
    }
    void SpawnPortals()
    {
        int id = 0;

        foreach (var spawnPoint in SpawnPoints)
        {
            Vector3 spawnPos = spawnPoint.transform.position + new Vector3(0, 0.2f, 0);
            Quaternion lookRotation = Quaternion.identity;

            if (id % 2 == 1) lookRotation = Quaternion.Euler(0, 90f, 0);

            Portal portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity, transform);

            portal.id = id;
            portal.SpawnTile = spawnPoint;
            portal.transform.rotation = portal.SpawnTile.PortalRotationHelper();

            allPortals.Add(portal);

            id++;
        }
    }
    int GetPortalCountForWave(int wave)
    {
        if (wave < 3) return 1;
        if (wave < 6) return 2;
        if (wave < 9) return 3;
        return 4;
    }
    public bool HasActivePortal() => allPortals.Any(p => !p.IsDestroyed);
    public void Clear()
    {
        if (ActivePortals == null) return;
        foreach (var portal in ActivePortals)
        {
            portal.SpawnTile.Restore();
        }
        ActivePortals.Clear();
    }
    public void CalculateActivePortals(int waveIndex)
    {
        if (allPortals == null)
        {
            Debug.LogWarning("null");
            return;
        }
        ActivePortals.Clear();

        numberOfActivePortals = GetPortalCountForWave(waveIndex);

        var available = allPortals
        .Where(p => !p.IsDestroyed)
            .OrderBy(x => Random.value)
                .Take(numberOfActivePortals);

        ActivePortals = available.ToList();
        DrawEnemyPath();
    }
    public void DrawEnemyPath()
    {
        if (ActivePortals == null) return;

        foreach (var portal in ActivePortals)
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
    public Portal GetAvailablePortal(int startIndex)
    {
        if (ActivePortals.Count == 0) return null;

        for (int i = 0; i < ActivePortals.Count; i++)
        {
            int index = (startIndex + i) % ActivePortals.Count;
            if (!ActivePortals[index].IsDestroyed)
                return ActivePortals[index];
        }

        return null;
    }
    public void OnPortalDestroy(Portal portal)
    {
        if (ActivePortals.Contains(portal))
        {
            portal.SpawnTile.Restore();
            DestroyPath(portal);
            ActivePortals.Remove(portal);

        }
        SyncActivePortals();
    }
    void FindMissingPortals()
    {
        int missing = numberOfActivePortals - ActivePortals.Count;

        if (missing <= 0) return;
        var candidates = allPortals
            .Where(p => !p.IsDestroyed && !ActivePortals.Contains(p))
            .OrderBy(x => Random.value)
            .Take(missing)
            .ToList();

        if (candidates.Count > 0)
        {
            foreach (var portal in candidates)
            {
                ActivePortals.Add(portal);
            }
            DrawEnemyPath();
        }
    }
    void SyncActivePortals()
    {
        bool anyPortalLeft = allPortals.Any(p => !p.IsDestroyed);

        if (currentState == GameState.Passive)
        {
            FindMissingPortals();
            if (!anyPortalLeft)
                OnAllPortalsDestroyed?.Invoke();
        }
        if (currentState == GameState.Active)
        {
            if (!anyPortalLeft)
                OnAllPortalsDestroyed?.Invoke();
        }
    }
    void CheckAllPortals()
    {
        if (!allPortals.Any(p => !p.IsDestroyed)) OnAllPortalsDestroyed?.Invoke();
    }
    public void DestroyPath(Portal portal)
    {
        if (portal.path == null) return;
        foreach (var path in portal.path)
        {
            Destroy(path);
        }
        portal.path.Clear();
    }

    private void OnEnable()
    {
        StateManager.Instance.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (StateManager.Instance != null)
            StateManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    void HandleStateChanged(GameState state)
    {
        currentState = state;
    }
    #region Save
    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);
    public string GetUniqueSaveID()
    {
        return nameof(PortalManager);
    }
    public ISaveData SaveState()
    {
        GeneralData saveData = new GeneralData();
        saveData.activePortalIDs.Clear();
        foreach (var portal in ActivePortals) saveData.activePortalIDs.Add(portal.ID);
        return saveData;
    }
    public void LoadState(ISaveData data)
    {
        GeneralData saveData = data as GeneralData;
        ActivePortals.Clear();
        foreach (var id in saveData.activePortalIDs)
        {
            if (portalsByID.TryGetValue(id, out Portal portal)) ActivePortals.Add(portal);
        }
    }
    #endregion
}
