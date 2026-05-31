using System;
using System.Collections.Generic;
using UnityEngine;

public class QualityViewModel : IViewModel
{
    private readonly ISettingsRepository _repository;
    private readonly IQualityService _service;

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
        _service = qualityService;

        Load();
    }
    private void Load()
    {
        qualityLevels = _service.GetAllLevels(); // to define the default one;

        int defaultIndex = _service.GetDefaultQualityIndex();
        if (!_repository.HasQualitySave())
        {
            var bootstrapped = QualitySettingsModel.CreateDefault(defaultIndex);
            _repository.SaveQuality(bootstrapped);
        }
        CurrentData = _repository.LoadQuality(defaultIndex);
        _service.Apply(CurrentData);
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
        _service.Apply(CurrentData);
        _repository.SaveQuality(CurrentData);
        OnChanged?.Invoke();
    }
    public void ResetToDefault()
    {
        int defaultIndex = _service.GetDefaultQualityIndex();
        PendingData = QualitySettingsModel.CreateDefault(defaultIndex);
        //OnChanged?.Invoke();
        Apply();
    }
    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();
        OnChanged?.Invoke();
    }
}
