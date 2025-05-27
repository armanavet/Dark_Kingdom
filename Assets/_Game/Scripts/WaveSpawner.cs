using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] Unit[] units;
    [SerializeField] Wave[] waves;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] int spawnerDestroyTime;
    [SerializeField] int spawnDistanceFromCenter;
    [SerializeField, FloatRangeSliderAttribute(-10f, 10f)] FloatRange distanceVariance = new FloatRange(0f);

    GameObject spawner;
    Wave currentWave;
    public int EnemyCount;
    #region Singleton 
    private static WaveSpawner _instance;
    public static WaveSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WaveSpawner>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public void SpawnWave(int wave)
    {
        if (units == null || units.Length == 0) return;
        if (waves == null || waves.Length == 0) return;
        if (wave >= waves.Length) return;

        currentWave = waves[wave];
        Tile spawnPoint = CalculateSpawnPoint();
        spawner = Instantiate(spawnerPrefab, spawnPoint.transform.localPosition, spawnPoint.pathDirection.GetRotation());
        StartCoroutine(SpawnUnits(spawnPoint));
    }

    Tile CalculateSpawnPoint()
    {
        List<Tile> tiles = GameBoard.Instance.tiles;
        List<Tile> potentialPoints = new List<Tile>();
        int variance = (int)distanceVariance.RandomValueInRange;

        foreach (var tile in tiles)
        {
            if (tile.DistanceToDestinationOriginal == spawnDistanceFromCenter + variance)
            {
                potentialPoints.Add(tile);
            }
        }

        Tile randomPoint = potentialPoints[Random.Range(0, potentialPoints.Count - 1)];
        return randomPoint;
    }

    IEnumerator SpawnUnits(Tile spawnPoint)
    {
        foreach (var unit in currentWave.Units)
        {
            int unitCount = unit.Count;
            GameObject unitPrefab = units.Where(x => x.Type == unit.Type).Select(x => x.Model).FirstOrDefault();
            for (int i = 0; i < unitCount; i++)
            {
                GameObject enemy = Instantiate(unitPrefab, spawnPoint.transform.position, spawnPoint.pathDirection.GetRotation());
                enemy.GetComponent<Enemy>().OnSpawn(spawnPoint, 0f);
                EnemyCount++;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        Destroy(spawner, spawnerDestroyTime);
    }
}

#region Units
public enum UnitType
{
    hayvan
}

[System.Serializable]
public class Unit
{
    public GameObject Model;
    public UnitType Type;
}
#endregion

#region Wave

[System.Serializable]
public class Wave
{
    public UnitToSpawn[] Units;
}

[System.Serializable]
public class UnitToSpawn
{
    public UnitType Type;
    public int Count;
}

#endregion
