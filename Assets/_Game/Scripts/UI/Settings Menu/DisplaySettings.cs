using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.Collections.Generic;
public class DisplaySettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown, screenModeDropdown;
    [Range(1f, 2f)][SerializeField] private float minAspectRatioSize = 1.5f;
    private List<Resolution> filteredResolutions;

    private List<string> screenModes = new List<string>()
    { "Exclusive Fullscreen", "Fullscreen", "Windowed" };

    private SettingsViewModel viewModel;
    public void InitializeData(SettingsInstaller installer)
    {
        viewModel = installer.ViewModel;
        
        resolutionDropdown.ClearOptions();
        screenModeDropdown.ClearOptions();

        filteredResolutions = installer.DisplayService.GetFilteredResolutions();
        var options = filteredResolutions
        .Select(r => $"{r.width} x {r.height} @ {r.refreshRateRatio.value:F0}Hz")
        .ToList();
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        screenModeDropdown.AddOptions(screenModes);
        screenModeDropdown.RefreshShownValue();
        
        resolutionDropdown.onValueChanged.AddListener(viewModel.SetResolution);
        screenModeDropdown.onValueChanged.AddListener(viewModel.SetScreenMode);

        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();

        viewModel.OnChanged += Refresh;
        Refresh();
    }
    void Refresh()
    {
        resolutionDropdown.SetValueWithoutNotify(viewModel.PendingDisplayData.ResolutionIndex);
        screenModeDropdown.SetValueWithoutNotify(viewModel.PendingDisplayData.ScreenModeIndex);

        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();
    }
}
