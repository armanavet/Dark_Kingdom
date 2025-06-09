using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type;

    [SerializeField] Transform arrow;
    [SerializeField] GameObject NeutralTile, OwnTile, ObstructedTile;
    [SerializeField] Tile north, south, east, west, nextOnPath;
    [SerializeField] int distanceToDestination = 0;
    public Vector2Int coordinates;
    public Direction pathDirection;
    public Vector3 exitPoint;
    public bool isEmpty = true;
    public int TilePrice;

    public Tile NextOnPath => nextOnPath;
    public int DistanceToDestinationOriginal { get; private set; }
    public int Index => GameBoard.Instance.Length * coordinates.y + coordinates.x;
    bool canBePath => (Type == TileType.Neutral ||
                       Type == TileType.Own ||
                       Type == TileType.Claimed) &&
                       distanceToDestination == int.MaxValue;

    public List<Tile> neighbors = new List<Tile>();

    public Tile GrowPathNorth(bool ignoreTowers) => GrowPathTo(north, Direction.South, ignoreTowers);
    public Tile GrowPathSouth(bool ignoreTowers) => GrowPathTo(south, Direction.North, ignoreTowers);
    public Tile GrowPathEast(bool ignoreTowers) => GrowPathTo(east, Direction.West, ignoreTowers);
    public Tile GrowPathWest(bool ignoreTowers) => GrowPathTo(west, Direction.East, ignoreTowers);

    private void Start()
    {
        arrow.gameObject.SetActive(false);
    }

    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }
    public void SetType(TileType type)
    {
        Type = type;

        NeutralTile.SetActive(false);
        OwnTile.SetActive(false);
        ObstructedTile.SetActive(false);

        switch (type)
        {
            case TileType.Neutral:
                NeutralTile.SetActive(true);
                break;
            case TileType.Own:
                OwnTile.SetActive(true);
                break;
            case TileType.Claimed:
                OwnTile.SetActive(true);
                break;
            case TileType.Obstructed:
                ObstructedTile.SetActive(true);
                break;
            default: break;
        }
    }

    public void SetNeighbors()
    {
        neighbors = new List<Tile>()
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
        if (nextTile == null || !nextTile.canBePath) return null;

        if (!ignoreTowers && !nextTile.isEmpty) return null;


        arrow.gameObject.SetActive(true);
        nextTile.arrow.gameObject.SetActive(true);
        nextTile.distanceToDestination = distanceToDestination + 1;

        nextTile.nextOnPath = this;
        nextTile.arrow.rotation = Quaternion.LookRotation(nextTile.arrow.forward, (arrow.position - nextTile.arrow.position).normalized);
        nextTile.pathDirection = direction;
        nextTile.exitPoint = nextTile.transform.position + direction.GetHalfVector();

        if (nextTile.DistanceToDestinationOriginal == 0)
        {
            nextTile.DistanceToDestinationOriginal = distanceToDestination + 1;
        }
        return nextTile;
    }

    public void ClaimNeighbors()
    {
        foreach (var neighbor in neighbors)
        {
            if (neighbor == null || neighbor.Type != TileType.Neutral) continue;
            
            neighbor.SetType(TileType.Claimed);
        }
    }

    public void UnclaimNeighbors()
    {
        foreach (var neighbor in neighbors)
        {
            if (neighbor == null || neighbor.Type != TileType.Claimed) continue;

            bool canUnclaim = true;
            List<Tile> surroundingTiles = neighbor.neighbors;
            foreach (var tile in surroundingTiles)
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

        foreach (var neighbor in neighbors)
        {
            if (neighbor.Type == TileType.Own || neighbor.Type == TileType.Claimed)
                return true;
        }
        return false;
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
    Neutral,    //White
    Own,        //Yellow
    Claimed,    //Yellow
    Obstructed, //Black
    Destination,//None
}
