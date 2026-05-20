using System.Collections.Generic;
using UnityEngine;

public class QualityService : IQualityService
{
    private readonly QualityConfigRegistry qualityConfigRegistry;
    public QualityService(QualityConfigRegistry qualityConfigRegistry)
    {
        this.qualityConfigRegistry = qualityConfigRegistry;
    }
    //add filtered quality list
    //add shadow and other configs schanging system.
    public int GetDefaultQualityIndex() => QualitySettings.GetQualityLevel();
    public IReadOnlyList<QualityLevelConfig> GetAllLevels() => qualityConfigRegistry.GetAllLevels();
    public void Apply(QualitySettingsModel settings)
    {
        if (settings.QualityIndex >= QualitySettings.names.Length)
        {
            Debug.LogWarning($"Quality index {settings.QualityIndex} has no matching Unity quality tier.");
            return;
        }
        QualitySettings.SetQualityLevel(settings.QualityIndex, false);
    }
}
