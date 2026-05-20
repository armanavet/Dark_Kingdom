using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayViewModel : IViewModel
{
    private readonly ISettingsRepository repository;
    private readonly IDisplayService service;
    public DisplaySettingsModel CurrentData { get; private set; }
    public DisplaySettingsModel PendingData { get; private set; }

    public List<Resolution> filteredResolutions { get; private set; }

    public bool HasChanges => 
        PendingData.ResolutionIndex != CurrentData.ResolutionIndex ||
        PendingData.ScreenModeIndex != CurrentData.ScreenModeIndex;
    public event Action OnChanged;
    public DisplayViewModel(
        ISettingsRepository repository,
        IDisplayService service) 
    {
        this.repository = repository;   
        this.service = service;

        Load();
    }
    void Load()
    {
        filteredResolutions = service.GetFilteredResolutions();
        int defaultResIndex = service.GetDefaultResolutionIndex(filteredResolutions);
        int defaultModeIndex = service.GetDefaultScreenModeIndex();

        CurrentData = repository.LoadDisplay(defaultResIndex, defaultModeIndex);
        PendingData = CurrentData.Clone();
    }

    public void SetResolution(int index)
    {
        PendingData.ResolutionIndex = index;
        OnChanged?.Invoke();
    }
    public void SetScreenMode(int index)
    {
        PendingData.ScreenModeIndex = index;
        OnChanged?.Invoke();
    }
    public List<Resolution> GetResolutions() => service.GetFilteredResolutions();
    public void Apply()
    {
        CurrentData = PendingData.Clone();

        service.Apply(CurrentData);

        repository.SaveDisplay(CurrentData);
        OnChanged?.Invoke();
    }
    
    public void ResetToDefault()
    {
        int defaultResIndex = service.GetDefaultResolutionIndex(filteredResolutions);
        int defaultModeIndex = service.GetDefaultScreenModeIndex();

        PendingData = DisplaySettingsModel.CreateDefault(defaultResIndex, defaultModeIndex);
        
        Apply();
    }

    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();
        
        OnChanged?.Invoke();
    }
}
