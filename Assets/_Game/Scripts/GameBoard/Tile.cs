using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type;

    //[SerializeField] Transform arrow;
    [SerializeField] GameObject[] NeutralTiles, OwnTiles, ObstructedTiles, ObstructedRiverTiles;
    [SerializeField] Tile north, south, east, west, nextOnPath;
    [SerializeField] Color regularColor, corruptedColor;
    [SerializeField] int distanceToDestination = 0;
    [SerializeField] GameObject currentModel;
    public Vector2Int coordinates;
    public Direction pathDirection;
    public Vector3 exitPoint;
    public bool isEmpty = true;
    public int TilePrice;
    public List<Tile> surroundingTiles = new List<Tile>();
    List<Tile> neighbors = new List<Tile>();

    public Tile NextOnPath => nextOnPath;
    public int DistanceToDestinationOriginal { get; private set; }
    public int Index => GameBoard.Instance.Length * coordinates.y + coordinates.x;
    bool canBePath => (Type == TileType.Neutral ||
                       Type == TileType.Own ||
                       Type == TileType.Bridge);
    bool canBeRiverPath => (Type == TileType.Obstructed_River || Type == TileType.Bridge);
    public Tile GrowPathNorth(bool ignoreTowers) => GrowPathTo(north, Direction.South, ignoreTowers);
    public Tile GrowPathSouth(bool ignoreTowers) => GrowPathTo(south, Direction.North, ignoreTowers);
    public Tile GrowPathEast(bool ignoreTowers) => GrowPathTo(east, Direction.West, ignoreTowers);
    public Tile GrowPathWest(bool ignoreTowers) => GrowPathTo(west, Direction.East, ignoreTowers);

    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }
    public void SetType(TileType type, bool setModel = true)
    {
        Type = type;
        if (setModel) SetModel();
    }
    public void SetModel()
    {
        NeutralTiles.ToList().ForEach(x => x.SetActive(false));
        OwnTiles.ToList().ForEach(x => x.SetActive(false));
        ObstructedTiles.ToList().ForEach(x => x.SetActive(false));
        ObstructedRiverTiles.ToList().ForEach(x => x.SetActive(false));

        int random;
        switch (Type)
        {
            case TileType.Neutral:
            case TileType.Bridge:
                Connect(NeutralTiles);
                break;
            case TileType.Own:
            case TileType.Destination:
                if (OwnTiles.Length > 0)
                {
                    random = Random.Range(0, OwnTiles.Length);
                    currentModel = OwnTiles[random];
                }
                break;
            case TileType.Obstructed:
                if (ObstructedTiles.Length > 0)
                {
                    random = Random.Range(0, ObstructedTiles.Length);
                    currentModel = ObstructedTiles[random];
                }
                break;
            case TileType.Obstructed_River:
                Connect(ObstructedRiverTiles, true);
                break;
            default: break;
        }
        currentModel.SetActive(true);
    }
    void Connect(GameObject[] tilesModels, bool isRiver = false)
    {
        if (tilesModels == null || tilesModels.Length == 0) return;

        GameObject straight = null, corner = null, end = null, tSection = null, crossroads = null, bridge = null;

        if (isRiver == false && tilesModels.Length == NeutralTiles.Length)
        {
            straight = NeutralTiles[0];
            corner = NeutralTiles[1];
            end = NeutralTiles[2];
            tSection = NeutralTiles[3];
            crossroads = NeutralTiles[4];
            bridge = NeutralTiles[5];
        }
        else if (ObstructedRiverTiles.Length == ObstructedRiverTiles.Length)
        {
            straight = ObstructedRiverTiles[0];
            corner = ObstructedRiverTiles[1];
            end = ObstructedRiverTiles[2];
        }
        else
        {
            Debug.Log("You're trying to connect the wrong tiles!!!");
        }

        // Get neighbors that matter (river vs road)
        List<Tile> neighborsList = neighbors
            .Where(x => x != null && (isRiver ? x.canBeRiverPath : x.canBePath))
            .ToList();

        int count = neighborsList.Count;

        //Fallback if not enough models
        if (tilesModels.Length <= count)
        {
            ActivateModel(tilesModels[0]);
            return;
        }

        switch (count)
        {
            case 0:
                currentModel = end; // end tile fallback
                break;

            case 1:
                currentModel = end; // end
                LookAtTile(neighborsList[0].transform);
                break;

            case 2:
                if (VectorOperations.PointsLineUp(neighborsList[0].coordinates, neighborsList[1].coordinates))
                {
                    if (isRiver == false && IsBridgeCase())
                    {
                        currentModel = bridge;
                        currentModel.transform.rotation = SetBridgeRotation();
                    }
                    else
                    {
                        currentModel = straight;
                        LookAtTile(neighborsList[0].transform);
                    }
                }
                else
                {
                    currentModel = corner;
                    currentModel.transform.rotation = SetCornerRotation();
                }
                break;

            case 3:
                currentModel = tSection; // T-section
                currentModel.transform.rotation = SetTSectionRotation();
                break;

            case 4:
                currentModel = crossroads; // Crossroad
                break;
        }

        ActivateModel(currentModel);
    }
    Quaternion SetCornerRotation()
    {
        float yRotation = 0;
        if (Type == TileType.Neutral)
        {
            if (north != null && east != null && north.canBePath && east.canBePath) yRotation = 90f;
            else if (east != null && south != null && east.canBePath && south.canBePath) yRotation = 180f;
            else if (south != null && west != null && south.canBePath && west.canBePath) yRotation = 270f;
        }
        else if (Type == TileType.Obstructed_River)
        {
            if (north != null && east != null && north.canBeRiverPath && east.canBeRiverPath) yRotation = 90f;
            else if (east != null && south != null && east.canBeRiverPath && south.canBeRiverPath) yRotation = 180f;
            else if (south != null && west != null && south.canBeRiverPath && west.canBeRiverPath) yRotation = 270f;
        }
        return Quaternion.Euler(0, yRotation, 0);
    }
    Quaternion SetTSectionRotation()
    {
        float yRotation = 0;
        if (west == null || !west.canBePath) yRotation = 90f;
        else if (north == null || !north.canBePath) yRotation = 180f;
        else if (east == null || !east.canBePath) yRotation = 270f;

        return Quaternion.Euler(0, yRotation, 0);
    }
    Quaternion SetBridgeRotation()
    {
        float yRotation = 0;
        if (north != null && south != null && north.canBePath && south.canBePath) yRotation = 90f;
        else if (west != null && east != null && west.canBePath && east.canBePath) yRotation = 0;

        return Quaternion.Euler(0, yRotation, 0);
    }
    bool IsBridgeCase()
    {
        bool eastWest = west?.canBeRiverPath == true && east?.canBeRiverPath == true;
        bool northSouth = north?.canBeRiverPath == true && south?.canBeRiverPath == true;
        return eastWest || northSouth;
    }
    void ActivateModel(GameObject model)
    {
        currentModel = model;
        currentModel.SetActive(true);
    }
    void LookAtTile(Transform target)
    {
        currentModel.transform.LookAt(target);
    }
    public void SetSurroundingTiles()
    {
        surroundingTiles = new List<Tile>()
        {
            north,
            north?.east,
            east,
            east?.south,
            south,
            south?.west,
            west,
            west?.north
        };
    }
    public void SetNeighbors()
    {
        if (north != null) neighbors.Add(north);
        if (east != null) neighbors.Add(east);
        if (south != null) neighbors.Add(south);
        if (west != null) neighbors.Add(west);
    }
    public void MakeEastWestConnection(Tile east, Tile west)
    {
        east.west = west;
        west.east = east;
    }
    public void MakeNorthSouthConnection(Tile north, Tile south)
    {
        north.south = south;
        south.north = north;
    }
    public void BecomeDestination()
    {
        distanceToDestination = 0;
        DistanceToDestinationOriginal = 0;
        nextOnPath = null;
        exitPoint = transform.localPosition;
        isEmpty = false;
    }
    public void ClearPath()
    {
        distanceToDestination = int.MaxValue;
        nextOnPath = null;
    }
    Tile GrowPathTo(Tile nextTile, Direction direction, bool ignoreTowers)
    {
        if (nextTile == null || !nextTile.canBePath || nextTile.distanceToDestination != int.MaxValue) return null;

        if (!ignoreTowers && !nextTile.isEmpty) return null;

        //arrow.gameObject.SetActive(true);
        //nextTile.arrow.gameObject.SetActive(true);
        nextTile.distanceToDestination = distanceToDestination + 1;
        nextTile.nextOnPath = this;
        //nextTile.arrow.rotation = Quaternion.LookRotation(nextTile.arrow.forward, (arrow.position - nextTile.arrow.position).normalized);
        nextTile.pathDirection = direction;
        nextTile.exitPoint = nextTile.transform.position + direction.GetHalfVector();

        if (nextTile.DistanceToDestinationOriginal == 0)
        {
            nextTile.DistanceToDestinationOriginal = distanceToDestination + 1;
        }
        return nextTile;
    }
    public void ClaimSurroundingTiles()
    {
        foreach (var neighbor in surroundingTiles)
        {
            if (neighbor.Type == TileType.Obstructed) neighbor.SetType(TileType.Own);
        }
    }
    public void Corrupt()
    {
        currentModel.GetComponent<Renderer>().material.color = corruptedColor;
        foreach (var neighbor in surroundingTiles)
        {
            neighbor.currentModel.GetComponent<Renderer>().material.color = corruptedColor;
            //if (neighbor.Type == TileType.Obstructed) neighbor.SetType(TileType.Claimed_Obstructed);
            //else if (neighbor.Type == TileType.Own) neighbor.SetType(TileType.Claimed_Own);
        }
    }
    public void Restore()
    {
        currentModel.GetComponent<Renderer>().material.color = regularColor;
        foreach (var neighbor in surroundingTiles)
        {
            neighbor.currentModel.GetComponent<Renderer>().material.color = regularColor;
            //if (neighbor.Type == TileType.Claimed_Obstructed) neighbor.SetType(TileType.Obstructed);
            //else if (neighbor.Type == TileType.Claimed_Own) neighbor.SetType(TileType.Own);
        }
    }
    public TileData OnSave()
    {
        return new TileData(Type, DistanceToDestinationOriginal);
    }
    public void OnLoad(TileData data)
    {
        Type = data.Type;
        DistanceToDestinationOriginal = data.DistanceToDestinationOriginal;
    }
}
public enum TileType
{
    Neutral,
    Bridge,
    Own,
    Claimed_Obstructed,
    Claimed_Own,
    Obstructed,
    Obstructed_River,
    Destination
}
