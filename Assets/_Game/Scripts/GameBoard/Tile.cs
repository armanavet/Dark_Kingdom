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
    public int? DistanceToDestinationOriginal { get; private set; }
    public Tile NextOnPath => nextOnPath;
    public Vector2Int coordinates;
    public Vector3 exitPoint;
    public Direction pathDirection;
    public bool isEmpty = true; 
    bool isPath => distanceToDestination != int.MaxValue;

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
            case TileType.Obstructed:
                ObstructedTile.SetActive(true);
                break;

            default: break;
        }
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
    }

    public void ClearPath()
    {
        distanceToDestination = int.MaxValue;
        nextOnPath = null;
    }

    Tile GrowPathTo(Tile nextTile, Direction direction, bool ignoreTowers)
    {
        if (nextTile == null || (nextTile.Type != TileType.Neutral && nextTile.Type != TileType.Own) || nextTile.isPath) return null;
        if (!ignoreTowers && !nextTile.isEmpty) return null;

        arrow.gameObject.SetActive(true);
        nextTile.arrow.gameObject.SetActive(true);
        nextTile.distanceToDestination = distanceToDestination + 1;
        nextTile.DistanceToDestinationOriginal ??= distanceToDestination + 1;
        nextTile.nextOnPath = this;
        nextTile.arrow.rotation = Quaternion.LookRotation(nextTile.arrow.forward, (arrow.position - nextTile.arrow.position).normalized);
        nextTile.pathDirection = direction;
        nextTile.exitPoint = nextTile.transform.position + direction.GetHalfVector();
        return nextTile;
    }
}

public enum TileType
{
    Neutral,   //White
    Own,       //Yellow  
    Obstructed,//Black
    Destination//None
}
