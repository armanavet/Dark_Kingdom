using System;
using UnityEngine;

public class AudioViewModel : IViewModel
{
    private readonly ISettingsRepository repository;
    private readonly IAudioService service;
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
        this.repository = repository;
        this.service = service;

        Load();
    }
    void Load()
    {
        CurrentData = repository.LoadAudio();

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

        service.Apply(CurrentData);

        repository.SaveAudio(CurrentData);

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
