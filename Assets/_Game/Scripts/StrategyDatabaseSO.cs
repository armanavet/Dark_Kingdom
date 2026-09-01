using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Strategy Database", fileName="StrategyDB")]
public class StrategyDatabaseSO : ScriptableObject
{
    [SerializeField] private Strategy[] strategies;
    private Dictionary<StrategyType, Strategy> strategiesByType;

    private void OnEnable()
    {
        SortByType();
    }

    private void OnValidate()
    {
        SortByType();
    }

    private void SortByType()
    {
        strategiesByType ??= new();

        foreach (var s in strategies)
        {
            if (s == null) continue;

            strategiesByType[s.Type] = s;
        }
    }

    public Strategy GetByType(StrategyType type)
    {
        strategiesByType.TryGetValue(type, out Strategy strategy);
        return strategy;
    }
}

public enum StrategyType
{
    Construction,
    Battle,
    Economy,
}

[Serializable]
public class Strategy
{
    public StrategyType Type;
    public float Cooldown;
    public Sprite Icon;
}
