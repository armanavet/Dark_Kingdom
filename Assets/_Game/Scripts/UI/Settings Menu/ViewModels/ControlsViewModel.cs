using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsViewModel : IViewModel
{
    private readonly ISettingsRepository repository;
    private readonly IInputService service;

    public ControlsSettingsModel CurrentData { get; private set; }
    public ControlsSettingsModel PendingData { get; private set; }
    public bool IsRebinding => service.IsRebinding;
    public bool HasChanges =>
        PendingData.MovementSpeed != CurrentData.MovementSpeed ||
        PendingData.RotationSpeed != CurrentData.RotationSpeed ||
        PendingData.ZoomSpeed != CurrentData.ZoomSpeed ||
        PendingData.DragSpeed != CurrentData.DragSpeed ||
        PendingData.RebindJson != CurrentData.RebindJson;
    public event Action OnChanged;

    public ControlsViewModel(
        ISettingsRepository repository,
        IInputService service)
    {
        this.repository = repository;
        this.service = service;

        Load();
    }

    void Load()
    {
        CurrentData = repository.LoadControls();

        service.LoadBindings(CurrentData.RebindJson);

        PendingData = CurrentData.Clone();
    }

    public void SetMoveSpeed(float value)
    {
        PendingData.MovementSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetRotationSpeed(float value)
    {
        PendingData.RotationSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetZoomSpeed(float value)
    {
        PendingData.ZoomSpeed = value;
        OnChanged?.Invoke();
    }

    public void SetDragSpeed(float value)
    {
        PendingData.DragSpeed = value;
        OnChanged?.Invoke();
    }

    public void Apply()
    {
        CurrentData = PendingData.Clone();

        service.LoadBindings(CurrentData.RebindJson);

        repository.SaveControls(CurrentData);

        OnChanged?.Invoke();
    }
    public void RevertToSaved()
    {
        PendingData = CurrentData.Clone();

        service.LoadBindings(CurrentData.RebindJson);

        OnChanged?.Invoke();
    }
    public void ResetToDefault()
    {
        service.ResetAllBindingsToDefault();

        PendingData = ControlsSettingsModel.CreateDefault(service.SaveBindings());

        Apply();
    }
    public void StartRebind(InputActionId actionId, int bindingIndex)
    {
        if (IsRebinding) return;

        var op = service.StartRebind(actionId, bindingIndex);
        if (op == null) return;

        op.OnFinished += result =>
        {
            PendingData.RebindJson = service.SaveBindings();
            OnChanged?.Invoke();
        };

        OnChanged?.Invoke();
    }
    public void ResetBindingToDefault(InputActionId actionId, int bindingIndex)
    {
        service.ResetBindingToDefault(actionId, bindingIndex);

        PendingData.RebindJson = service.SaveBindings();

        OnChanged?.Invoke();
    }
    public string GetBindingDisplay(InputActionId actionId, int bindingIndex)
    {
        string path = service.GetBindingPath(actionId, bindingIndex);
        if (BindingPathToKeyConverter.TryGetKey(path, out Key key))
        {
            return KeyDisplayMap.Get(key);
        }
        return InputControlPath.ToHumanReadableString(path);
    }
    public void CancelRebind() => service.CancelRebind();
}

