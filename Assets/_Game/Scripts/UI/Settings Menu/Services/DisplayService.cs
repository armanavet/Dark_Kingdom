using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Linq;
public class DisplayService : IDisplayService
{
    private Resolution[] resolutions;
    private readonly float minAspectRatio = 1.5f;
    private readonly FullScreenMode[] modes =
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };
    public List<Resolution> GetFilteredResolutions()
    {
        resolutions = Screen.resolutions;
        var result = new List<Resolution>();

        foreach (var resolution in resolutions)
        {
            if (resolution.width == 640 && resolution.height == 480)
            {
                if (!result.Any(
                    x => x.width == resolution.width &&
                    x.height == resolution.height &&
                    x.refreshRateRatio.value == resolution.refreshRateRatio.value))
                {
                    result.Add(resolution);
                }
            }


            float aspectRatio = (float)resolution.width / resolution.height;

            if (aspectRatio >= minAspectRatio)
            {
                if (!result.Any(
                    x => x.width == resolution.width &&
                    x.height == resolution.height &&
                    x.refreshRateRatio.value == resolution.refreshRateRatio.value))
                {
                    result.Add(resolution);
                }
            }
        }
        if (result != null)
            return result;
        else return null;
    }
    public int GetCurrentResolutinIndex(List<Resolution> filtered)
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
    public int GetCurrentScreenModeIndex()
    {
        for (int i = 0; i < modes.Length; i++)
        {
            if (Screen.fullScreenMode == modes[i])
                return i;
        }
        return 1;
    }
    public void Apply(DisplaySettingsModel settings)
    {
        var resolutions = Screen.resolutions;
        var res = resolutions[settings.ResolutionIndex];

        Screen.SetResolution(
            res.width,
            res.height,
            modes[settings.ScreenModeIndex],
            res.refreshRateRatio);
    }


}
