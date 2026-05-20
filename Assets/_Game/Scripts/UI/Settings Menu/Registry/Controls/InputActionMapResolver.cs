using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class InputActionMapResolver
{
    private readonly Dictionary<InputActionId, InputAction> _map;

    public InputActionMapResolver(InputActionAsset asset)
    {
        _map = new Dictionary<InputActionId, InputAction>
        {
            { InputActionId.MoveForward, FindRequiredAction(asset,"MoveForward") },
            { InputActionId.MoveBackward, FindRequiredAction(asset,"MoveBackward") },
            { InputActionId.MoveLeft, FindRequiredAction(asset,"MoveLeft") },
            { InputActionId.MoveRight, FindRequiredAction(asset,"MoveRight") },
            //{ InputActionId.CameraRotate, _asset.FindAction("CameraRotate") },
            //{ InputActionId.Zoom, _asset.FindAction("Zoom") },
            //{ InputActionId.Drag, _asset.FindAction("Drag") }
        };
    }
    public InputAction Resolve(InputActionId id) => _map[id];

    private InputAction FindRequiredAction(InputActionAsset asset, string name)
    {
        var action = asset.FindAction(name);

        if (action == null)
            throw new Exception($"Missing action: {name}");

        return action;
    }
}
