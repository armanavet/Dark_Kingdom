using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [HideInInspector] public int Length;
    [HideInInspector] public int Width;
    public List<Tile>  tiles = new List<Tile>();
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
        Initialize();
        BuildPathToDestination();
    }

    void Initialize()
    {
        for (int i = 0, y = 0; y < tiles.Count / Width; y++)
        {
            for (int x = 0; x < tiles.Count / Length ;x++, i++)
            {
                if (x > 0)
                {
                    tiles[i].MakeEastWestConnection(tiles[i], tiles[i - 1]);
                }
                if (y > 0)
                {
                    tiles[i].MakeNorthSouthConnection(tiles[i], tiles[i - Length]);
                }
            }
        }
    }

    public void BuildPathToDestination()
    {
        foreach (Tile tile in tiles)
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

            SearchFrontier.Enqueue(tile.GrowPathNorth());
            SearchFrontier.Enqueue(tile.GrowPathSouth());
            SearchFrontier.Enqueue(tile.GrowPathEast());
            SearchFrontier.Enqueue(tile.GrowPathWest());
        }
    }
}
