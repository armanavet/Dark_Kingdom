using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public static class BindingPathToKeyConverter{
    public static bool TryGetKey(string bindingPath, out Key key)
    {
        key = default;

        var control = InputSystem.FindControl(bindingPath);

        if (control is KeyControl keyControl)
        {
            key = keyControl.keyCode;
            return true;
        }
        return false;
    }
    
}
