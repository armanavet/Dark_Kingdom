using AudioSystem;
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class SourceLibrary<T> : ScriptableObject
    where T : Enum
{
    [SerializeField] List<SourceEntry<T>> entries;

    Dictionary<T, SoundData> dictionary;
    void OnEnable()
    {
        dictionary = new Dictionary<T, SoundData>();

        foreach (var entry in entries)
        {
            if (entry.Type == null)
                continue;

            dictionary[entry.Type] = entry.Data;
        }
    }

    public SoundData Get(T type)
    {
        if (!dictionary.TryGetValue(type, out var data))
        {
            Debug.LogError($"Source data for {type} not found.");
            return null;
        }

        return data;
    }
}

[CreateAssetMenu(fileName = "SourceBank", menuName = "ScriptableObjects/SourceBank")]
public class SourceBank : ScriptableObject
{
    public SourceDataSO so;
    public List<SourceSOEntry<UnitType>> enemy;
    public List<SourceSOEntry<TowerType>> tower;

    Dictionary<UnitType, SourceDataSO> enemyDict = new Dictionary<UnitType, SourceDataSO>();
    Dictionary<TowerType, SourceDataSO> towerDict = new Dictionary<TowerType, SourceDataSO>();
    public void Build()
    {
        enemyDict.Clear();
        towerDict.Clear();

        foreach (var item in enemy)
        {
            enemyDict.Add(item.Type, item.Entry);
        }
        foreach (var item in tower)
        {
            towerDict.Add(item.Type, item.Entry);
        }

    }
    public SoundData GetData(SoundDataType type)
    {
        if (so != null)
        {
            return so.Get(type);
        }

        Debug.LogWarning($"No source registered");
        return null;
    }
    public SoundData GetData<T>(T source, SoundDataType type)
       where T : Enum
    {
        if (source is UnitType unitType && enemyDict.TryGetValue(unitType, out var enemy))
        {
            return enemy.Get(type);
        }
        if (source is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
        {
            return tower.Get(type);
        }
        Debug.LogWarning($"I got the wrong source type '{source}',or the wrong data type '{type}'");
        return null;
    }
}
