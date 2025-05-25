using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    float ColorTolerance = 0.1f;

    #region Singleton 
    private static Test _instance;
    public static Test Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Test>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    public void MakeNeighboursYellow(int x, int y, List<Tile> tiles, int width, int height, TileType tileType)
    {
        Tile tile;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int neighborX = x + dx;
                int neighborY = y + dy;

                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    int index = neighborY * width + neighborX;
                    tile = tiles[index];
                    if (tile.Type == TileType.Neutral)
                        continue;
                    tile.Type = TileType.Own;
                    tile.SetType(tileType);
                }
            }
        }
    }
    public TileType GetTileType(Color color)
    {
        if (isColorClose(color, Color.white)) return TileType.Neutral;
        if (isColorClose(color, Color.yellow)) return TileType.Own;
        if (isColorClose(color, Color.black)) return TileType.Obstructed;
        return TileType.Neutral;
    }
    bool isColorClose(Color a, Color b)
    {
        return (Mathf.Abs(a.r - b.r) < ColorTolerance && Mathf.Abs(a.g - b.g) < ColorTolerance && Mathf.Abs(a.b - b.b) < ColorTolerance);
    }
}
