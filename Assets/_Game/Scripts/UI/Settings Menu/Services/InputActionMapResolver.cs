using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.VersionControl;

public class InputActionMapResolver
{
    private Dictionary<InputActionId, InputAction> _map;

    public InputActionMapResolver(InputActionAsset asset)
    {
        _map = new Dictionary<InputActionId, InputAction>
        {
            { InputActionId.MoveForward, asset.FindAction("MoveForward") },
            { InputActionId.MoveBackward, asset.FindAction("MoveBackward") },
            { InputActionId.MoveLeft, asset.FindAction("MoveLeft") },
            { InputActionId.MoveRight, asset.FindAction("MoveRight") },
            //{ InputActionId.CameraRotate, _asset.FindAction("CameraRotate") },
            //{ InputActionId.Zoom, _asset.FindAction("Zoom") },
            //{ InputActionId.Drag, _asset.FindAction("Drag") }
        };
    }
    public InputAction Resolve(InputActionId id) => _map[id];

}
