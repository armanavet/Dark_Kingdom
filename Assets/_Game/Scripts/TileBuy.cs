using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuy : MonoBehaviour 
{
    [HideInInspector]public Tile tile;
    
    public void ButtonBuyTile()
    {
        EconomyManager.Instance.ChangeGoldAmount(-tile.TilePrice);
        tile.SetType(TileType.Own);
        GameBoard.Instance.BuildPathToDestination(true);
        this.gameObject.SetActive(false);
    }
}
