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
    public Tile NextOnPath => nextOnPath;
    public Vector2Int coordinates;
    public Direction pathDirection;
    public Vector3 exitPoint;
    public bool isEmpty = true; 
    bool isPath => distanceToDestination != int.MaxValue;

    public Tile GrowPathNorth() => GrowPathTo(this.north, Direction.South);
    public Tile GrowPathSouth() => GrowPathTo(this.south, Direction.North);
    public Tile GrowPathEast() => GrowPathTo(this.east, Direction.West);
    public Tile GrowPathWest() => GrowPathTo(this.west, Direction.East);

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
        nextOnPath = null;
        exitPoint = transform.localPosition;
    }

    public void ClearPath()
    {
        distanceToDestination = int.MaxValue;
        nextOnPath = null;
    }

    Tile GrowPathTo(Tile nextTile, Direction direction)
    {
        if (nextTile == null || (nextTile.Type != TileType.Neutral && nextTile.Type != TileType.Own) || nextTile.isPath) return null;

        arrow.gameObject.SetActive(true);
        nextTile.arrow.gameObject.SetActive(true);
        nextTile.distanceToDestination = distanceToDestination + 1;
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
