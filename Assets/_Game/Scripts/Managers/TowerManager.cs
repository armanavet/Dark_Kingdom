using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour, ISaveable
{
    public Tower[] TowerPrefabs;
    public TowerPreview[] TowerPreviews;
    Dictionary<TowerType, Tower> prefabsByType = new Dictionary<TowerType, Tower>();
    Dictionary<TowerType, TowerPreview> previewsByType = new Dictionary<TowerType, TowerPreview>();
    public List<Tower> Towers;

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
    }
    #endregion

    void Start()
    {
        foreach (var prefab in TowerPrefabs)
            prefabsByType.Add(prefab.Type, prefab);

        foreach (var preview in TowerPreviews)
            previewsByType.Add(preview.Type, preview);

        SaveManager.RegisterSaveable(this);
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
        tile.ClaimNeighbors();
        Towers.Add(tower);

        return tower;
    }

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
        for (int i = 0; i < saveData.Count; i++)
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
