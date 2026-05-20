using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsViewModel
{
    private readonly ISettingsRepository _repository;       
    private readonly IDisplayService _displayService;
    private readonly IAudioService _audioService;
    private readonly IInputService _inputService;

    public DisplaySettingsModel CurrentDisplayData { get; private set; }
    public AudioSettingsModel CurrentAudioData { get; private set; }
    public ControlsSettingsModel CurrentControlsData { get; private set; }
    public DisplaySettingsModel PendingDisplayData { get; private set; }
    public AudioSettingsModel PendingAudioData { get; private set; }
    public ControlsSettingsModel PendingControlsData { get; private set; }

    public List<Resolution> FilteredResolutions { get; private set; }

    //public event Action OnRebindStarted;
    //public event Action OnRebindCompleted;

    //static int res_value, mode_value;
    //static float master_value = ConstValues.DEFAULT_MASTER_VOLUME;
    //static float music_value = ConstValues.DEFAULT_MUSIC_VOLUME;
    //static float sfx_value = ConstValues.DEFAULT_SFX_VOLUME;
    public bool IsRebinding => _inputService.IsRebinding;
    public bool HasChanges =>
        PendingDisplayData.ResolutionIndex != CurrentDisplayData.ResolutionIndex ||
        PendingDisplayData.ScreenModeIndex != CurrentDisplayData.ScreenModeIndex ||
        PendingAudioData.Master != CurrentAudioData.Master ||
        PendingAudioData.Music != CurrentAudioData.Music ||
        PendingAudioData.SFX != CurrentAudioData.SFX || PendingControlsData.MovementSpeed != CurrentControlsData.MovementSpeed ||
        PendingControlsData.RotationSpeed != CurrentControlsData.RotationSpeed ||
        PendingControlsData.ZoomSpeed != CurrentControlsData.ZoomSpeed ||
        PendingControlsData.DragSpeed != CurrentControlsData.DragSpeed ||
        PendingControlsData.RebindJson != CurrentControlsData.RebindJson;


    public event Action OnChanged;
    public SettingsViewModel(
        ISettingsRepository repository,
        IDisplayService displayService,
        IAudioService audioService,
        IInputService inputService)
    {
        _repository = repository;
        _displayService = displayService;
        _audioService = audioService;
        _inputService = inputService;

        Load();
    }
    void Load()
    {
        FilteredResolutions = _displayService.GetFilteredResolutions();
        int defaultResIndex = _displayService.GetDefaultResolutionIndex(FilteredResolutions);
        int defaultModeIndex = _displayService.GetDefaultScreenModeIndex();

        CurrentDisplayData = _repository.LoadDisplay(defaultResIndex, defaultModeIndex);
        CurrentAudioData = _repository.LoadAudio();
        CurrentControlsData = _repository.LoadControls();

        _inputService.LoadBindings(CurrentControlsData.RebindJson);

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

    public void Apply()
    {
        CurrentDisplayData = PendingDisplayData.Clone();
        CurrentAudioData = PendingAudioData.Clone();
        CurrentControlsData = PendingControlsData.Clone();

        _displayService.Apply(CurrentDisplayData);
        _audioService.Apply(CurrentAudioData);
        _inputService.LoadBindings(CurrentControlsData.RebindJson);

        //_repository.Save(CurrentDisplayData, CurrentAudioData, CurrentControlsData);

        OnChanged?.Invoke();
    }
    public void RevertToSaved()
    {
        PendingDisplayData = CurrentDisplayData.Clone();
        PendingAudioData = CurrentAudioData.Clone();
        PendingControlsData = CurrentControlsData.Clone();

        _inputService.LoadBindings(CurrentControlsData.RebindJson);

        OnChanged?.Invoke();
    }
    public void ResetToDefault()
    {
        int defaultResIndex = _displayService.GetDefaultResolutionIndex(FilteredResolutions);
        int defaultModeIndex = _displayService.GetDefaultScreenModeIndex();

        PendingDisplayData = DisplaySettingsModel.CreateDefault(defaultResIndex, defaultModeIndex);
        PendingAudioData = AudioSettingsModel.CreateDefault();

        _inputService.ResetAllBindingsToDefault();
        PendingControlsData = ControlsSettingsModel.CreateDefault(_inputService.SaveBindings());

        Apply();
    }
    public void StartRebind(InputActionId actionId, int bindingIndex)
    {
        if (IsRebinding) return;

        var op = _inputService.StartRebind(actionId, bindingIndex);
        if (op == null) return;
        op.OnFinished += result =>
        {
            PendingControlsData.RebindJson = _inputService.SaveBindings();

            OnChanged?.Invoke();
        };

        OnChanged?.Invoke();
    }
    public void ResetBindingToDefault(InputActionId actionId, int bindingIndex)
    {
        _inputService.ResetBindingToDefault(actionId, bindingIndex);

        PendingControlsData.RebindJson = _inputService.SaveBindings();

        OnChanged?.Invoke();
    }
    public string GetBindingDisplay(InputActionId actionId, int bindingIndex)
        => _inputService.GetBindingPath(actionId, bindingIndex);
    public void CancelRebind() => _inputService.CancelRebind();


}
public enum InputActionId
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
}