using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public string TowerName;
    public int TowerPrice;
    public int GoldGenerated;
    public string ButtonTag;
    public TowerType Type;
    public Button TowerButton;

}

public enum TowerType
{
    MainTower,
    ArcherTower,
    WizardTower,
    ArtilleryTower,
    GoldMine
}
