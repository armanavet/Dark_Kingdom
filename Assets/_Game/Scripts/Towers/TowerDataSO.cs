using UnityEngine;
using System;
using AudioSystem;

[CreateAssetMenu(menuName = "ScriptableObjects/Tower Data", fileName = "Tower Data")]
public class TowerDataSO : ScriptableObject
{
    [field: SerializeField] public TowerType Type {get; private set;}
    [field: SerializeField] public Faction Faction {get; private set;}
    [field: SerializeField] public int PurchasePrice {get; private set;}
    [field: SerializeField] public float Cooldown {get; private set;}
    [field: SerializeField] public float Range {get; private set;}
    [field: SerializeField] public int TargetPriority {get; private set;}
    [field: SerializeField] public Transform BuildEffectHigh {get; private set;}
    [field: SerializeField] public Transform BuildEffectLow {get; private set;}
    [field: SerializeField] public Transform UpgradeEffect {get; private set;}
    [field: SerializeField] public TowerLevel[] Levels { get; private set; }
}

[Serializable]
public class TowerLevel
{
    [field: SerializeField] public GameObject Projectile {get; private set;}
    [field: SerializeField] public Debuff Debuff {get; private set;}
    [field: SerializeField] public int UpgradePrice {get; private set;}
    [field: SerializeField] public int SellPrice {get; private set;}
    [field: SerializeField] public float HP {get; private set;}
    [field: SerializeField] public float Damage {get; private set;}
    [field: SerializeField] public int CrystalsGenerated {get; private set;}
    [field: SerializeField] public float CanvasHeight { get; private set; }
}
