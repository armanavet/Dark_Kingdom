using System;
using UnityEngine;

public class SettingsViewModel
{
    private readonly ISettingsRepository repository;
    private readonly IDisplayService displayService;
    private readonly IAudioService audioService;

    public DisplaySettingsModel CurrentDisplayData { get; private set; }
    public AudioSettingsModel CurrentAudioData { get; private set; }
    public DisplaySettingsModel PendingDisplayData { get; private set; }
    public AudioSettingsModel PendingAudioData { get; private set; }

    public event Action OnChanged;

    static int res_value, mode_value;
    static float master_value = ConstValues.DEFAULT_MASTER_VOLUME;
    static float music_value = ConstValues.DEFAULT_MUSIC_VOLUME;
    static float sfx_value = ConstValues.DEFAULT_SFX_VOLUME;
    public bool HasChanges =>
        PendingDisplayData.ResolutionIndex != CurrentDisplayData.ResolutionIndex ||
        PendingDisplayData.ScreenModeIndex != CurrentDisplayData.ScreenModeIndex ||
        PendingAudioData.Master != CurrentAudioData.Master ||
        PendingAudioData.Music != CurrentAudioData.Music ||
        PendingAudioData.SFX != CurrentAudioData.SFX;

    public SettingsViewModel(
        ISettingsRepository repository,
        IDisplayService displayService,
        IAudioService audioService)
    {
        this.repository = repository;
        this.displayService = displayService;
        this.audioService = audioService;

        Load();
    }
    void Load()
    {
        var filtered = displayService.GetFilteredResolutions();
        int res_value = displayService.GetCurrentResolutinIndex(filtered);
        int mode_value = displayService.GetCurrentScreenModeIndex();

        CurrentDisplayData = repository.LoadDesplay(res_value, mode_value);
        CurrentAudioData = repository.LoadAudio();

        PendingDisplayData = CurrentDisplayData.Clone();
        PendingAudioData = CurrentAudioData.Clone();
    }
    // ------ called by UI ------ \\

    public void SetResolution(int index)
    {
        PendingDisplayData.ResolutionIndex = index;
        OnChanged?.Invoke();
    }

    public void SetScreenMode(int index)
    {
        PendingDisplayData.ScreenModeIndex = index;
        OnChanged?.Invoke();
    }

    public void SetMaster(float value)
    {
        PendingAudioData.Master = value;
        OnChanged?.Invoke();
    }

    public void SetMusic(float value)
    {
        PendingAudioData.Music = value;
        OnChanged?.Invoke();
    }

    public void SetSFX(float value)
    {
        PendingAudioData.SFX = value;
        OnChanged?.Invoke();
    }

    // ------ Commands ------ \\

    public void Apply()
    {
        CurrentDisplayData = PendingDisplayData.Clone();
        CurrentAudioData = PendingAudioData.Clone();

        displayService.Apply(CurrentDisplayData);
        audioService.Apply(CurrentAudioData);

        repository.Save(CurrentDisplayData, CurrentAudioData);

        OnChanged?.Invoke();
    }

    public void ResetToDefault()
    {
        PendingDisplayData = new DisplaySettingsModel
        {
            ResolutionIndex = res_value,
            ScreenModeIndex = mode_value
        };
        PendingAudioData = new AudioSettingsModel
        {
            Master = master_value,
            Music = music_value,
            SFX = sfx_value
        };

        OnChanged?.Invoke();
    }

    public void DenyChanges()
    {
        PendingDisplayData = CurrentDisplayData.Clone();
        PendingAudioData = CurrentAudioData.Clone();
        OnChanged?.Invoke();
    }
}
