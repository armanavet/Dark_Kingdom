using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DisplayService : IDisplayService
{
    private readonly float minAspectRatio;
    private readonly FullScreenMode[] modes =
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };
    public DisplayService(float aspectRatio) 
    {
        minAspectRatio = aspectRatio;
    }
    public List<Resolution> GetFilteredResolutions()
    {
        var resolutions = Screen.resolutions;
        var result = new List<Resolution>();

        foreach (Resolution resolution in resolutions)
        {
            bool is640x480 = resolution.width == 640 && resolution.height == 480;
            bool isWideEnough = (float)resolution.width / resolution.height >= minAspectRatio;

            if (!is640x480 && !isWideEnough)
                continue;

            if (!IsDuplicate(result, resolution))
                result.Add(resolution);
        }
        return result;
    }
    public int GetDefaultResolutionIndex(List<Resolution> filtered)
    {
        for (int i = 0; i < filtered.Count; i++)
        {
            if (filtered[i].width == Screen.width &&
                filtered[i].height == Screen.height &&
                Mathf.RoundToInt((float)filtered[i].refreshRateRatio.value) ==
                Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value))
            {
                return i;
            }
        }
        return 0;
    }
    public int GetDefaultScreenModeIndex()
    {
        for (int i = 0; i < modes.Length; i++)
        {
            if (Screen.fullScreenMode == modes[i]) 
            {
                return i;
            }
        }
        return 1; // default: FullScreenWindow
    }
    public void Apply(DisplaySettingsModel settings)
    {
        List<Resolution> filtered = GetFilteredResolutions();
        
        if(settings.ResolutionIndex >= filtered.Count) return;

        Resolution res = filtered[settings.ResolutionIndex];

        Screen.SetResolution(
            res.width,
            res.height,
            modes[settings.ScreenModeIndex],
            res.refreshRateRatio);
    }
    private static bool IsDuplicate(List<Resolution> list, Resolution candidate)
    {
        return list.Any(x =>
            x.width == candidate.width &&
            x.height == candidate.height &&
            x.refreshRateRatio.value == candidate.refreshRateRatio.value);
    }
    
}

//foreach (var resolution in resolutions)
//{
//    if (resolution.width == 640 && resolution.height == 480)
//    {
//        if (!result.Any(
//            x => x.width == resolution.width &&
//            x.height == resolution.height &&
//            x.refreshRateRatio.value == resolution.refreshRateRatio.value))
//        {
//            result.Add(resolution);
//        }
//    }


//    float aspectRatio = (float)resolution.width / resolution.height;

//    if (aspectRatio >= minAspectRatio)
//    {
//        if (!result.Any(
//            x => x.width == resolution.width &&
//            x.height == resolution.height &&
//            x.refreshRateRatio.value == resolution.refreshRateRatio.value))
//        {
//            result.Add(resolution);
//        }
//    }
//}
//if (result != null)
//    return result;