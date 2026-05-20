using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class ControlsService : IInputService
{
    private readonly InputActionAsset _actions;
    private readonly InputActionMapResolver _resolver;
    private RebindOperation _activeOperation;
    public bool IsRebinding => _activeOperation != null;

    public ControlsService(InputActionAsset actions)
    {
        _actions = actions;
        _resolver = new InputActionMapResolver(actions);

        _actions.FindActionMap("Gameplay").Enable();
    }
    public RebindOperation StartRebind(InputActionId actionId, int bindingIndex)
    {
        if (IsRebinding) return null;

        var action = _resolver.Resolve(actionId);
        _activeOperation = new RebindOperation(action, bindingIndex);

        _activeOperation.OnFinished += _ => _activeOperation = null;

        return _activeOperation;
    }
    public void CancelRebind() => _activeOperation?.Cancel();

    public string GetBindingPath(InputActionId actionId, int bindingIndex)
        => _resolver.Resolve(actionId).bindings[bindingIndex].effectivePath;

    public void LoadBindings(string json)
    {
        if (!string.IsNullOrEmpty(json))
            _actions.LoadBindingOverridesFromJson(json);
    }

    public void ResetAllBindingsToDefault()
    {
        foreach (var map in _actions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }
    public void ResetBindingToDefault(InputActionId actionId, int bindingIndex)
    {   
        var action = _resolver.Resolve(actionId);
        action.RemoveBindingOverride(bindingIndex);
    }

    public string SaveBindings() => _actions.SaveBindingOverridesAsJson();
}
