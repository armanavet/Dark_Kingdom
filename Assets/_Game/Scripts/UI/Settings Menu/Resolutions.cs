using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Resolutions : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [Range(1f, 2f)][SerializeField] private float minAspectRatioSize = 1.5f;

    private Resolution[] resolutions;
    
    private List<string> screenModes = new List<string>()
    { "Exclusive Fullscreen", "Fullscreen", "Windowed" };
    
    private readonly FullScreenMode[] modes = new[]
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };
    
    private List<Resolution> filteredResolutions;
    private Resolution selectedResolution;
    private int currentResolutionIndex = 0;
    private int previousResolutionIndex, previousSceenModeIndex;
    //TO DO
    //Make save and reset system.
    public int ResolutionIndex;
    public int SceenModeIndex;
    void Start()
    {
        InitializeResolution();
        InitializeScreenMode();

        ApplyDisplaySettings();
    }
    void InitializeResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();

        foreach (var resolution in resolutions)
        {
            if (resolution.width == 640 && resolution.height == 480)
            {
                if (!filteredResolutions.Any(
                    x => x.width == resolution.width &&
                    x.height == resolution.height &&
                    x.refreshRateRatio.value == resolution.refreshRateRatio.value))
                {
                    filteredResolutions.Add(resolution);
                }
            }


            float aspectRatio = (float)resolution.width / resolution.height;
            Debug.Log($"Aspect Ratio = {aspectRatio}");

            if (aspectRatio >= minAspectRatioSize)
            {
                if (!filteredResolutions.Any(
                    x => x.width == resolution.width &&
                    x.height == resolution.height &&
                    x.refreshRateRatio.value == resolution.refreshRateRatio.value))
                {
                    filteredResolutions.Add(resolution);
                }
            }
        }

        if (filteredResolutions.Count == 0)
        {
            Debug.LogError("No valid reolution found!");
            return;
        }

        List<string> options = new List<string>();

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption =
                 $"{filteredResolutions[i].width} x {filteredResolutions[i].height} @ " +
                 $"{filteredResolutions[i].refreshRateRatio.value.ToString("F0")} Hz";

            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height &&
                Mathf.RoundToInt((float)filteredResolutions[i].refreshRateRatio.value) ==
                Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value))
            {
                currentResolutionIndex = i;
            }
        }

        previousResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        int savedResolution = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = Mathf.Clamp(savedResolution, 0, filteredResolutions.Count - 1);
        resolutionDropdown.RefreshShownValue();
    }
    public void InitializeScreenMode()
    {
        int savedMode = PlayerPrefs.GetInt("ScreenModeIndex", 1);
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(screenModes);
        screenModeDropdown.value = Mathf.Clamp(savedMode, 0, modes.Length - 1);
        previousSceenModeIndex = PlayerPrefs.GetInt("ScreenMode", screenModeDropdown.value);
        screenModeDropdown.RefreshShownValue();
    }
    void ApplyDisplaySettings()
    {
        if (filteredResolutions == null || filteredResolutions.Count == 0)
        {
            Debug.LogError("No valid resolution found!");
            return;
        }
        int resolutionIndex = Mathf.Clamp(resolutionDropdown.value, 0, filteredResolutions.Count - 1);
        int modeIndex = Mathf.Clamp(screenModeDropdown.value, 0, modes.Length - 1);

        selectedResolution = filteredResolutions[resolutionIndex];
        FullScreenMode mode = modes[modeIndex];


        if (Screen.width != selectedResolution.width ||
            Screen.height != selectedResolution.height ||
            Screen.fullScreenMode != mode)
        {
            Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            mode,
            selectedResolution.refreshRateRatio
            );

            PlayerPrefs.SetInt("ScreenModeIndex", modeIndex);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.Save();
        }
    }
    public void ApplyResolution() => ApplyDisplaySettings();
    public void ApplyScreenMode() => ApplyDisplaySettings();

}
