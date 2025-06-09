using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    Dictionary<string, ISaveData> data = new Dictionary<string, ISaveData>();

    public void Add(string id, ISaveData saveData) => data[id] = saveData;
    public ISaveData Get(string id) => data[id];
}

[System.Serializable]
public class GeneralData : ISaveData
{
    public int CurrentWave;
    public int CurrentGold;
    public int EnemySpawnTile;
}

[System.Serializable]
public class DataList<T> : ISaveData, IEnumerable<T>
{
    List<T> items = new List<T>();

    public T this[int index] => items[index];

    public int Count => items.Count;

    public void Add(T item)
    {
        items.Add(item);
    }

    public void Remove(T item)
    {
        items.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}

[System.Serializable]
public class TileData
{
    public TileType Type;
    public int DistanceToDestinationOriginal;

    public TileData(TileType type, int distanceToDestinationOriginal)
    {
        Type = type;
        DistanceToDestinationOriginal = distanceToDestinationOriginal;
    }
}

[System.Serializable]
public class TowerData
{
    public TowerType Type;
    public int TileIndex;
    public int Level;
    public float CurrentHP;

    public TowerData(TowerType type, int tileIndex, int level, float currentHP)
    {
        Type = type;
        TileIndex = tileIndex;
        Level = level;
        CurrentHP = currentHP;
    }
}
