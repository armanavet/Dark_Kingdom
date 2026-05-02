using System.Collections.Generic;
using UnityEngine;

public interface IDisplayService
{
    public List<Resolution> GetFilteredResolutions();
    public int GetCurrentResolutinIndex(List<Resolution> filtered);
    public int GetCurrentScreenModeIndex();
    void Apply(DisplaySettingsModel settings);
}
