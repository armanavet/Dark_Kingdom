using System.Collections.Generic;
using UnityEngine;

public interface IQualityService
{
    int GetDefaultQualityIndex();
    IReadOnlyList<QualityLevelConfig> GetAllLevels();
    void Apply(QualitySettingsModel quality);
}
