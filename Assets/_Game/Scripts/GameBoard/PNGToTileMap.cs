    using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class PNGToTileMap : MonoBehaviour
{
    public Texture2D sourceImage;
    public GameObject TilePrefab;
    public string savePrefabPath = "Assets/_Game/Prefabs/GameBoard.prefab";
    GameObject GridParent;
    float ColorTolerance = 0.1f;
    public void GenerateGrid()
    {
        int Width = sourceImage.width;
        int Height = sourceImage.height;
        GridParent = new GameObject("GameBoard");
        GameBoard script = GridParent.AddComponent<GameBoard>();
        script.Length = Height;
        script.Width = Width;
        script.Tiles = new List<Tile>();
        Vector2 offset = new Vector2((Width - 1) * 0.5f, (Height - 1) * 0.5f);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color pixelColor = sourceImage.GetPixel(x, y);
                TileType TileType = GetTileType(pixelColor);

                GameObject tileObj = Instantiate(TilePrefab, GridParent.transform);
                tileObj.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
                tileObj.name = $"Tile_{x}_{y}";

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.SetType(TileType, false);
                    tile.SetCoordinates(x,y);
                    script.Tiles.Add(tile);
                }
            }
        }

        GameBoard.Instance.Initialize();

    }
    TileType GetTileType(Color color)
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

    public void SaveAsPrefab()
    {
        if(GridParent == null)
        {
            Debug.LogError("Parent is null");
            return;
        }
        PrefabUtility.SaveAsPrefabAsset(GridParent, savePrefabPath);
        Debug.Log("map prefab saved succesfully");
    }
}


[CustomEditor(typeof(PNGToTileMap))]
public class PNGToTileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PNGToTileMap pNGToTileMap = (PNGToTileMap)target;

        if(GUILayout.Button("Generate Grid"))
        {
            pNGToTileMap.GenerateGrid();
        }
        if (GUILayout.Button("Save Prefab"))
        {
            pNGToTileMap.SaveAsPrefab();
        }

    }
}