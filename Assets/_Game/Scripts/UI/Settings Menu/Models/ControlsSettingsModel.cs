using UnityEngine;

public class ControlsSettingsModel
{
    public string RebindJson; // froom InputSystem 
    public float MovementSpeed;
    public float RotationSpeed;
    public float ZoomSpeed;
    public float DragSpeed;

    public ControlsSettingsModel Clone()
    {
        return new ControlsSettingsModel
        {
            RebindJson = RebindJson,
            MovementSpeed = MovementSpeed,
            RotationSpeed = RotationSpeed,
            ZoomSpeed = ZoomSpeed,
            DragSpeed = DragSpeed
        };
    }
}
