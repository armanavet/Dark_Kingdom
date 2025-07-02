using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour, ISaveable
{
    public Tower[] TowerPrefabs;
    public TowerPreview[] TowerPreviews;
    Dictionary<TowerType, Tower> prefabsByType = new Dictionary<TowerType, Tower>();
    Dictionary<TowerType, TowerPreview> previewsByType = new Dictionary<TowerType, TowerPreview>();
    [HideInInspector]public List<Tower> Towers;
    [SerializeField] Tile mainTowerTile;

    #region Singleton 
    private static TowerManager _instance;
    public static TowerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TowerManager>();
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
        foreach (var prefab in TowerPrefabs)
            prefabsByType.Add(prefab.Type, prefab);

        foreach (var preview in TowerPreviews)
            previewsByType.Add(preview.Type, preview);

        BuildTower(TowerType.MainTower,mainTowerTile);
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
        Transform parentObject = FindObjectOfType<TowerManager>().transform;

        Tower tower = Instantiate(prefab, tile.transform.position, Quaternion.identity, parentObject);
        tower.tile = tile;
        tile.isEmpty = false;
        tile.ClaimSurroundingTiles();
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
    ArcherTower,
    WizardTower,
    ArtilleryTower,
    GoldMine,
    MainTower
}
