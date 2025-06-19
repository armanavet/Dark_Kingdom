using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour, ISaveable
{
    [HideInInspector] public int Length;
    [HideInInspector] public int Width;
    public List<Tile>  Tiles = new List<Tile>();
    Queue<Tile> SearchFrontier = new Queue<Tile>();

    #region Singleton 
    private static GameBoard _instance;
    public static GameBoard Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameBoard>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Start()
    {
        SaveManager.RegisterSaveable(this);
        Initialize();
    }

    void Initialize()
    {
        for (int i = 0, y = 0; y < Tiles.Count / Width; y++)
        {
            for (int x = 0; x < Tiles.Count / Length ;x++, i++)
            {
                if (x > 0)
                {
                    Tiles[i].MakeEastWestConnection(Tiles[i], Tiles[i - 1]);
                }
                if (y > 0)
                {
                    Tiles[i].MakeNorthSouthConnection(Tiles[i], Tiles[i - Length]);
                }
            }
        }

        foreach (var tile in Tiles)
        {
            tile.SetNeighbors();
        }
    }

    public void BuildPathToDestination(bool ignoreTowers)
    {
        foreach (Tile tile in Tiles)
        {
            tile.ClearPath();
            if (tile.Type == TileType.Destination)
            {
                tile.BecomeDestination();
                SearchFrontier.Enqueue(tile);
            }
        }

        while (SearchFrontier.Count > 0)
        {
            Tile tile = SearchFrontier.Dequeue();
            if (tile == null) continue;

            SearchFrontier.Enqueue(tile.GrowPathNorth(ignoreTowers));
            SearchFrontier.Enqueue(tile.GrowPathSouth(ignoreTowers));
            SearchFrontier.Enqueue(tile.GrowPathEast(ignoreTowers));
            SearchFrontier.Enqueue(tile.GrowPathWest(ignoreTowers));
        }
    }

    public string GetUniqueSaveID()
    {
        return nameof(GameBoard);
    }

    public ISaveData SaveState()
    {
        DataList<TileData> saveData = new DataList<TileData>();
        foreach (var tile in Tiles)
        {
            TileData tileData = tile.OnSave();
            saveData.Add(tileData);
        }
        return saveData;
    }

    public void LoadState(ISaveData data)
    {
        DataList<TileData> saveData = data as DataList<TileData>;
        for (int i = 0; i < saveData.Count; i++)
        {
            TileData tileData = saveData[i];
            Tiles[i].OnLoad(tileData);
        }
    }
}
