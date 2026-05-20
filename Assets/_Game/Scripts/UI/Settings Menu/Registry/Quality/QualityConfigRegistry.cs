using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

[CreateAssetMenu(fileName = "QualityConfigRegistry", menuName = "Settings/Quality Config Registry")]
public class QualityConfigRegistry : ScriptableObject
{
    [SerializeField] private List<QualityLevelConfig> levels;

    public int Count => levels.Count;

    public QualityLevelConfig Get(int index)
    {
        if (index < 0 || index > levels.Count)
        {
            Debug.LogWarning($"QualityConfigRegistry: index {index} out of range, returning first.");
            return levels[0];
        }
        return levels[index];
    }

    public IReadOnlyList<QualityLevelConfig> GetAllLevels() => levels;
}
