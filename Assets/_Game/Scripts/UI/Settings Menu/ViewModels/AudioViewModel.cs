using System;
using UnityEngine;

public class AudioViewModel : IViewModel
{
    private readonly ISettingsRepository _repository;
    private readonly IAudioService _service;
    public AudioSettingsModel CurrentData { get; private set; }
    public AudioSettingsModel PendingData { get; private set; }
    public bool HasChanges =>
    PendingData.Master != CurrentData.Master ||
    PendingData.Music != CurrentData.Music ||
    PendingData.SFX != CurrentData.SFX ||
    PendingData.UI != CurrentData.UI;

    public event Action OnChanged;
    public AudioViewModel(
    ISettingsRepository repository,
    IAudioService service)
    {
        _repository = repository;
        _service = service;

        Load();
    }
    void Load()
    {
        if (!_repository.HasAudioSave())
        {
            var bootstrapped = AudioSettingsModel.CreateDefault();
            _repository.SaveAudio(bootstrapped);
        }
        CurrentData = _repository.LoadAudio();
        _service.Apply(CurrentData);
        PendingData = CurrentData.Clone();
    }
    public void SetMaster(float value)
    {
        PendingData.Master = value;
        OnChanged?.Invoke();
    }
    public void SetMusic(float value)
    {
        PendingData.Music = value;
        OnChanged?.Invoke();
    }
    public void SetSFX(float value)
    {
        PendingData.SFX = value;
        OnChanged?.Invoke();
    }
    public void SetUI(float value)
    {
        PendingData.UI = value;
        OnChanged?.Invoke();
    }
    public void Apply()
    {
        CurrentData = PendingData.Clone();

        _service.Apply(CurrentData);

        _repository.SaveAudio(CurrentData);

        OnChanged?.Invoke();
    }
    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();

        OnChanged?.Invoke();
    }
    public void ResetToDefault()
    {
        PendingData = AudioSettingsModel.CreateDefault();

        Apply();
    }
}
