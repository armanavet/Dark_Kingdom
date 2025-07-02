using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type;

    //[SerializeField] Transform arrow;
    [SerializeField] GameObject[] NeutralTiles, OwnTiles, ObstructedTiles;
    [SerializeField] Tile north, south, east, west, nextOnPath;
    [SerializeField] Color regularColor, corruptedColor;
    [SerializeField] int distanceToDestination = 0;
    public Vector2Int coordinates;
    public Direction pathDirection;
    public Vector3 exitPoint;
    public bool isEmpty = true;
    public int TilePrice;
    public List<Tile> surroundingTiles = new List<Tile>();
    List<Tile> neighbors = new List<Tile>();
    [SerializeField] GameObject currentModel;

    public Tile NextOnPath => nextOnPath;
    public int DistanceToDestinationOriginal { get; private set; }
    public int Index => GameBoard.Instance.Length * coordinates.y + coordinates.x;
    bool canBePath => (Type == TileType.Neutral ||
                       Type == TileType.Own ||
                       Type == TileType.Claimed);

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

        int random;
        switch (Type)
        {
            case TileType.Neutral:
                ConnectRoads();
                break;
            case TileType.Own:
            case TileType.Claimed:
            case TileType.Destination:
                random = Random.Range(0, OwnTiles.Length);
                if (OwnTiles.Length > 0)
                    currentModel = OwnTiles[random];
                break;
            case TileType.Obstructed:
                random = Random.Range(0, ObstructedTiles.Length);
                if (ObstructedTiles.Length > 0)
                    currentModel = ObstructedTiles[random];
                break;
            default: break;
        }
        currentModel.SetActive(true);
    }

    void ConnectRoads()
    {
        if (NeutralTiles.Length == 0) return;

        if (NeutralTiles.Length < 5)
        {
            NeutralTiles[0].SetActive(true);
            return;
        }
        var straight = NeutralTiles[0];
        var corner = NeutralTiles[1];
        var tSection = NeutralTiles[2];
        var crossroads = NeutralTiles[3];
        var end = NeutralTiles[4];

        List<Tile> surroundingRoads = neighbors.Where(x => x.canBePath).ToList();
        switch (surroundingRoads.Count)
        {
            case 1:
                currentModel = end;
                currentModel.SetActive(true);
                currentModel.transform.LookAt(surroundingRoads[0].transform);
                break;
            case 2:
                if (VectorOperations.PointsLineUp(surroundingRoads[0].coordinates, surroundingRoads[1].coordinates))
                {
                    currentModel = straight;
                    currentModel.transform.LookAt(surroundingRoads[0].transform);
                }
                else
                {
                    currentModel = corner;
                    currentModel.SetActive(true);
                    currentModel.transform.rotation = SetCornerRotation();
                }
                break;
            case 3:
                currentModel = tSection;
                currentModel.SetActive(true);
                currentModel.transform.rotation = SetTSectionRotation();
                break;
            case 4:
                currentModel = crossroads;
                currentModel.SetActive(true);
                break;
            default:
                break;
        }
    }


    Quaternion SetCornerRotation()
    {
        float yRotation = 0;
        if (north != null && east != null && north.canBePath && east.canBePath) yRotation = 90f;
        else if (east != null && south != null && east.canBePath && south.canBePath) yRotation = 180f;
        else if (south != null && west != null && south.canBePath && west.canBePath) yRotation = 270f;

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
            if (neighbor == null || neighbor.Type != TileType.Neutral) continue;

            neighbor.SetType(TileType.Claimed);
        }
    }

    public void UnclaimSurroundingTiles()
    {
        foreach (var neighbor in surroundingTiles)
        {
            if (neighbor == null || neighbor.Type != TileType.Claimed) continue;

            bool canUnclaim = true;
            foreach (var tile in neighbor.surroundingTiles)
            {
                if (!tile.isEmpty)
                {
                    canUnclaim = false;
                    break;
                }
            }

            if (canUnclaim) neighbor.SetType(TileType.Neutral);
        }
    }

    public bool CanBePurchased()
    {
        if (Type != TileType.Obstructed) return false;

        foreach (var neighbor in surroundingTiles)
        {
            if (neighbor.Type == TileType.Own || neighbor.Type == TileType.Claimed)
                return true;
        }
        return false;
    }

    public void Corrupt()
    {
        currentModel.GetComponent<Renderer>().material.color = corruptedColor;
        foreach (var neighbor in surroundingTiles)
        {
            neighbor.currentModel.GetComponent<Renderer>().material.color = corruptedColor;
        }
    }

    public void Restore()
    {
        currentModel.GetComponent<Renderer>().material.color = regularColor;
        foreach (var neighbor in surroundingTiles)
        {
            neighbor.currentModel.GetComponent<Renderer>().material.color = regularColor;
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
    Own,
    Claimed,
    Obstructed,
    Destination
}
