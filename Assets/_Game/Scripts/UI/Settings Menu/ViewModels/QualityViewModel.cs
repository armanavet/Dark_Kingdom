using System;
using System.Collections.Generic;
using UnityEngine;

public class QualityViewModel : IViewModel
{
    private readonly ISettingsRepository _repository;
    private readonly IQualityService _qualityService;

    public QualitySettingsModel CurrentData { get; private set; }
    public QualitySettingsModel PendingData { get; private set; }
    public IReadOnlyList<QualityLevelConfig> qualityLevels { get; private set; }
    public bool HasChanges => PendingData.QualityIndex != CurrentData.QualityIndex;
    public event Action OnChanged;
    public QualityViewModel(
        ISettingsRepository repositroy,
        IQualityService qualityService)
    {
        _repository = repositroy;
        _qualityService = qualityService;

        Load();
    }
    private void Load()
    {
        qualityLevels = _qualityService.GetAllLevels(); // to define the default one;

        int defaultIndex = _qualityService.GetDefaultQualityIndex();

        CurrentData = _repository.LoadQuality(defaultIndex);
        PendingData = CurrentData.Clone();
    }
    public void SetQualityLevel(int index)
    {
        PendingData.QualityIndex = index;
        OnChanged?.Invoke();
    }
    public void Apply()
    {
        CurrentData = PendingData.Clone();
        _qualityService.Apply(CurrentData);
        _repository.SaveQuality(CurrentData);
        OnChanged?.Invoke();
    }
    public void ResetToDefault()
    {
        int defaultIndex = _qualityService.GetDefaultQualityIndex();
        PendingData = QualitySettingsModel.CreateDefault(defaultIndex);
        OnChanged?.Invoke();
    }
    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();
        OnChanged?.Invoke();
    }
}
