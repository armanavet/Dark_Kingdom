using System;
using UnityEngine;

public class SettingsViewModel
{
    private readonly ISettingsRepository repository;
    private readonly IDisplayService displayService;
    private readonly IAudioService audioService;
    private readonly IInputService inputService;

    public DisplaySettingsModel CurrentDisplayData { get; private set; }
    public AudioSettingsModel CurrentAudioData { get; private set; }
    public DisplaySettingsModel PendingDisplayData { get; private set; }
    public AudioSettingsModel PendingAudioData { get; private set; }
    public ControlsSettingsModel CurrentControlsData { get; private set; }
    public ControlsSettingsModel PendingControlsData { get; private set; }
    public bool IsRebinding => inputService.IsRebinding;
    //public InputActionId CurrentRebindingAction { get; private set; }

    public event Action OnChanged;
    public event Action OnRebindStarted;
    public event Action OnRebindCompleted;

    static int res_value, mode_value;
    static float master_value = ConstValues.DEFAULT_MASTER_VOLUME;
    static float music_value = ConstValues.DEFAULT_MUSIC_VOLUME;
    static float sfx_value = ConstValues.DEFAULT_SFX_VOLUME;
    public bool HasChanges =>
        PendingDisplayData.ResolutionIndex != CurrentDisplayData.ResolutionIndex ||
        PendingDisplayData.ScreenModeIndex != CurrentDisplayData.ScreenModeIndex ||
        PendingAudioData.Master != CurrentAudioData.Master ||
        PendingAudioData.Music != CurrentAudioData.Music ||
        PendingAudioData.SFX != CurrentAudioData.SFX ||
        PendingControlsData.MovementSpeed != CurrentControlsData.MovementSpeed ||
        PendingControlsData.RotationSpeed != CurrentControlsData.RotationSpeed ||
        PendingControlsData.ZoomSpeed != CurrentControlsData.ZoomSpeed ||
        PendingControlsData.DragSpeed != CurrentControlsData.DragSpeed ||
        PendingControlsData.RebindJson != CurrentControlsData.RebindJson;

    public SettingsViewModel(
        ISettingsRepository repository,
        IDisplayService displayService,
        IAudioService audioService,
        IInputService inputService)
    {
        this.repository = repository;
        this.displayService = displayService;
        this.audioService = audioService;
        this.inputService = inputService;

        Load();
    }
    void Load()
    {
        var filtered = displayService.GetFilteredResolutions();
        res_value = displayService.GetCurrentResolutinIndex(filtered);
        mode_value = displayService.GetCurrentScreenModeIndex();

        CurrentDisplayData = repository.LoadDesplay(res_value, mode_value);
        CurrentAudioData = repository.LoadAudio();
        CurrentControlsData = repository.LoadControls();

        inputService.LoadBindings(CurrentControlsData.RebindJson);

        PendingDisplayData = CurrentDisplayData.Clone();
        PendingAudioData = CurrentAudioData.Clone();
        PendingControlsData = CurrentControlsData.Clone();
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

    public void SetMoveSpeed(float value)
    {
        PendingControlsData.MovementSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetRotationSpeed(float value)
    {
        PendingControlsData.RotationSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetZoomSpeed(float value)
    {
        PendingControlsData.ZoomSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetDragSpeed(float value)
    {
        PendingControlsData.DragSpeed = value;
        OnChanged?.Invoke();

    }

    // ------ Commands ------ \\

    public void StartRebind(InputActionId actionId, int bindingIndex)
    {
        if (IsRebinding) return;

        var op = inputService.StartRebind(actionId, bindingIndex);
        if (op == null) return;
        op.OnFinished += result =>
        {
            PendingControlsData.RebindJson = inputService.SaveBindings();

            OnChanged?.Invoke();
        };

        OnChanged?.Invoke();
    }
    public void CancelRebind()
    {
        if (IsRebinding) return;

        inputService.CancelRebind();

        OnChanged?.Invoke();
    }
    public string GetBindingDisplay(InputActionId actionId, int bindingIndex)
    {
        return inputService.GetBindingDidplay(actionId, bindingIndex);
    }
    public void Apply()
    {
        CurrentDisplayData = PendingDisplayData.Clone();
        CurrentAudioData = PendingAudioData.Clone();
        CurrentControlsData = PendingControlsData.Clone();

        displayService.Apply(CurrentDisplayData);
        audioService.Apply(CurrentAudioData);

        inputService.LoadBindings(CurrentControlsData.RebindJson);

        repository.Save(CurrentDisplayData, CurrentAudioData,CurrentControlsData);

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

        inputService.ResetToDefault();

        PendingControlsData = new ControlsSettingsModel
        {
            RebindJson = inputService.SaveBindings(),
            MovementSpeed = 1f,
            RotationSpeed = 1f,
            ZoomSpeed = 1f,
            DragSpeed = 1f
        };

        OnChanged?.Invoke();
    }

    public void DenyChanges()
    {
        PendingDisplayData = CurrentDisplayData.Clone();
        PendingAudioData = CurrentAudioData.Clone();
        PendingControlsData = CurrentControlsData.Clone();

        inputService.LoadBindings(CurrentControlsData.RebindJson);

        OnChanged?.Invoke();
    }
}
public enum InputActionId
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
}