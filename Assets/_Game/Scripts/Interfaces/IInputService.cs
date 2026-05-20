using System;
using UnityEngine;

public interface IInputService
{
    bool IsRebinding { get; }
    RebindOperation StartRebind(InputActionId actionId, int bindingIndex);
    void CancelRebind();
    void ResetBindingToDefault(InputActionId actionId, int bindingIndex);
    void ResetAllBindingsToDefault();
    void LoadBindings(string json);
    string GetBindingPath(InputActionId actionId, int bindingIndex);
    string SaveBindings();
}
