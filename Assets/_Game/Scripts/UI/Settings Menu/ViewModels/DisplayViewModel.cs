using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayViewModel : IViewModel
{
    private readonly ISettingsRepository _repository;
    private readonly IDisplayService _service;
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
        _repository = repository;   
        _service = service;

        Load();
    }
    void Load()
    {
        filteredResolutions = _service.GetFilteredResolutions();
        int defaultResIndex = _service.GetDefaultResolutionIndex(filteredResolutions);
        int defaultModeIndex = _service.GetDefaultScreenModeIndex();

        if (!_repository.HasDisplaySave())
        {
            var bootstrapped = DisplaySettingsModel.CreateDefault(defaultResIndex, defaultModeIndex);
            _repository.SaveDisplay(bootstrapped);
        }

        CurrentData = _repository.LoadDisplay(defaultResIndex, defaultModeIndex);
            _service.Apply(CurrentData);
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
    public List<Resolution> GetResolutions() => _service.GetFilteredResolutions();
    public void Apply()
    {
        CurrentData = PendingData.Clone();

        _service.Apply(CurrentData);

        _repository.SaveDisplay(CurrentData);
        OnChanged?.Invoke();
    }
    
    public void ResetToDefault()
    {
        int defaultResIndex = _service.GetDefaultResolutionIndex(filteredResolutions);
        int defaultModeIndex = _service.GetDefaultScreenModeIndex();

        PendingData = DisplaySettingsModel.CreateDefault(defaultResIndex, defaultModeIndex);
        
        Apply();
    }

    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();
        
        OnChanged?.Invoke();
    }
}
