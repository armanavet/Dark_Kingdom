public class ControlsSettingsModel
{
    public const float DefaultSpeed = 1f;

    public string RebindJson; // froom InputSystem 
    public float MovementSpeed;
    public float RotationSpeed;
    public float ZoomSpeed;
    public float DragSpeed;

    public static ControlsSettingsModel CreateDefault(string rebindJson = "") => new ControlsSettingsModel()
    {
        RebindJson = rebindJson,
        MovementSpeed = DefaultSpeed,
        RotationSpeed = DefaultSpeed,
        ZoomSpeed = DefaultSpeed,
        DragSpeed = DefaultSpeed
    };
    public ControlsSettingsModel Clone() => new ControlsSettingsModel
    {
        RebindJson = RebindJson,
        MovementSpeed = MovementSpeed,
        RotationSpeed = RotationSpeed,
        ZoomSpeed = ZoomSpeed,
        DragSpeed = DragSpeed
    };

}
