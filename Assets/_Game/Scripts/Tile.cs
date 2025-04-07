using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameTileContentType Type;

    [SerializeField] GameObject NeutralTile, OwnTile, ObstructedTile;
    public bool isEmpty = true;


    public void SetType(GameTileContentType type)
    {
        Type = type;
        NeutralTile.SetActive(false);
        OwnTile.SetActive(false);
        ObstructedTile.SetActive(false);
        switch (type)
        {
            
            case GameTileContentType.Neutral:
                NeutralTile.SetActive(true);
                break;
            case GameTileContentType.Own:
                OwnTile.SetActive(true);
                break;
            case GameTileContentType.Obstructed:
                ObstructedTile.SetActive(true);
                break;

            default: break;
        }

        
    }
} 

public enum GameTileContentType
{
    Neutral,  //White
    Own,      //Yellow  
    Obstructed//Black
}
