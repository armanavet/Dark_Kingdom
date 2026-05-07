using System;
using UnityEngine;

public interface IInputService
{
    bool IsRebinding { get; }
    RebindOperation StartRebind(InputActionId actionId, int bindingIndex);
    void CancelRebind();
    string GetBindingDidplay(InputActionId actionId, int bindingIndex);
    void LoadBindings(string json);
    void ResetToDefault();
    string SaveBindings();
}
