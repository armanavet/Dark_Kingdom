using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Tower Descriptions DB", fileName = "Tower Descriptions")]
public class TowerDescriptionsSO : ScriptableObject
{
    [SerializeField] private TowerDescription[] descriptions;
    private Dictionary<TowerType, string> descriptionsByType = new();

    private void OnValidate()
    {
        foreach (var desc in descriptions)
        {
            descriptionsByType[desc.Tower] = desc.Description;
        }
    }

    public string GetByType(TowerType type)
    {
        return descriptionsByType[type];
    }
}

[Serializable]
public class TowerDescription
{
    public TowerType Tower;
    public string Description;
}
