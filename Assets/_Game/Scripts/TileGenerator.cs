using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.WSA;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int _mapSize;
    [SerializeField] Tile _tilePrefab;
    [SerializeField] float _scale;
    [SerializeField] bool PrimeryVersion;


    private void Start()
    {
        if (PrimeryVersion)
        {
            Initialize1();
        }
        else
        {
            Initialize2();
        }
    }

    void Initialize1()
    {
        float primeryNoise = 0;
        Vector2 offset = new Vector2((_mapSize.x - 1) * 0.5f, (_mapSize.y - 1) * 0.5f);
        for (int y = 0; y < _mapSize.y; y++)
        {
            for (int x = 0; x < _mapSize.x; x++)
            {
                Tile tile = Instantiate(_tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
                primeryNoise = Mathf.PerlinNoise(x* _scale, y* _scale);
                if (primeryNoise <= 0.15f)
                {
                    tile.SetType(GameTileContentType.Own);
                } else if (primeryNoise > 0.15f && primeryNoise < 0.6f)
                {
                    tile.SetType(GameTileContentType.Obstructed);
                }else
                {
                    tile.SetType(GameTileContentType.Neutral);
                }
            }
        }

    }

    void Initialize2()
    {
        float primeryNoise = 0;
        float secondaryNoise = 0;
        Vector2 offset = new Vector2((_mapSize.x - 1) * 0.5f, (_mapSize.y - 1) * 0.5f);
        for (int y = 0; y < _mapSize.y; y++)
        {
            for (int x = 0; x < _mapSize.x; x++)
            {
                Tile tile = Instantiate(_tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
                primeryNoise = Mathf.PerlinNoise(x * _scale, y * _scale);
                tile.SetType(primeryNoise < 0.5f ? GameTileContentType.Neutral : GameTileContentType.Obstructed);
                secondaryNoise = Mathf.PerlinNoise(x * _scale * 0.5f, y * _scale * 0.5f);
                if (secondaryNoise < 0.2f)
                {
                    tile.SetType(GameTileContentType.Own);
                }
            }
        }

    }


}
public enum GameTileContentType
{
    Neutral,  //White
    Own,      //Yellow  
    Obstructed//Black
}