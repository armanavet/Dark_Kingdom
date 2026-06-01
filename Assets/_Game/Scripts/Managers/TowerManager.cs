using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using AudioSystem;
using System;

public class TowerManager : MonoBehaviour, ISaveable
{
    public Tower[] TowerPrefabs;
    public TowerPreview[] TowerPreviews;
    public TowerDescriptionsSO TowerDescriptions;
    Dictionary<TowerType, Tower> prefabsByType = new Dictionary<TowerType, Tower>();
    Dictionary<TowerType, TowerPreview> previewsByType = new Dictionary<TowerType, TowerPreview>();
    [HideInInspector] public List<Tower> Towers;
    [SerializeField] Tile mainTowerTile;

    #region Singleton 
    private static TowerManager _instance;
    public static TowerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TowerManager>();
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
        foreach (var item in TowerPrefabs) prefabsByType[item.Type] = item;
        foreach (var item in TowerPreviews) previewsByType[item.Type] = item;

        BuildTower(TowerType.MainTower, mainTowerTile);
        mainTowerTile.SetType(TileType.Destination);
        foreach (var tile in mainTowerTile.surroundingTiles)
        {
            tile.SetType(TileType.Destination);
            tile.isEmpty = false;
        }
        GameBoard.Instance.BuildPathToDestination(false);
    }

    public Tower GetPrefabByType(TowerType type)
    {
        return prefabsByType[type];
    }

    public TowerPreview GetPreviewByType(TowerType type)
    {
        return previewsByType[type];
    }


    public Tower BuildTower(TowerType type, Tile tile)
    {
        Tower prefab = GetPrefabByType(type);
        Transform parentObject = FindFirstObjectByType<TowerManager>().transform;

        Tower tower = Instantiate(prefab, tile.transform.position, Quaternion.identity, parentObject);
        tower.Tile = tile;
        tile.isEmpty = false;
        if (type != TowerType.MainTower) tile.ClaimSurroundingTiles();
        Towers.Add(tower);
        return tower;
    }

    public void RegisterSaveable() => SaveManager.RegisterSaveable(this);

    public string GetUniqueSaveID()
    {
        return nameof(TowerManager);
    }

    public ISaveData SaveState()
    {
        DataList<TowerData> saveData = new DataList<TowerData>();
        foreach (var tower in Towers)
        {
            TowerData towerData = tower.OnSave();
            saveData.Add(towerData);
        }
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        DataList<TowerData> saveData = data as DataList<TowerData>;
        for (int i = 1; i < saveData.Count; i++)
        {
            TowerData towerData = saveData[i];
            Tower tower = BuildTower(towerData.Type, GameBoard.Instance.Tiles[towerData.TileIndex]);
            tower.OnLoad(towerData);
        }
    }
}

public enum TowerType
{
    Null,
    ArcherTower,
    WizardTower,
    ArtilleryTower,
    CrystalMine,
    MainTower
}
