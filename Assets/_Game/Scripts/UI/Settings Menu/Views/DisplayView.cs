using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
public class DisplayView: MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown, screenModeDropdown;
    private List<Resolution> filteredResolutions = new();

    private List<string> screenModes = new List<string>()
    { "Exclusive Fullscreen", "Fullscreen", "Windowed" };

    private DisplayViewModel viewModel;
    public void InitializeData(SettingsInstaller installer)
    {
        viewModel = installer.DisplayVM;
        
        resolutionDropdown.ClearOptions();
        screenModeDropdown.ClearOptions();

        filteredResolutions = installer.DisplayVM.filteredResolutions;
        Debug.Log(filteredResolutions);
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
        resolutionDropdown.SetValueWithoutNotify(viewModel.PendingData.ResolutionIndex);
        screenModeDropdown.SetValueWithoutNotify(viewModel.PendingData.ScreenModeIndex);

        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();
    }
}
