using System.Collections.Generic;
using UnityEngine;

public interface IDisplayService
{
    List<Resolution> GetFilteredResolutions();
    int GetDefaultResolutionIndex(List<Resolution> filtered);
    int GetDefaultScreenModeIndex();
    void Apply(DisplaySettingsModel settings);
}
