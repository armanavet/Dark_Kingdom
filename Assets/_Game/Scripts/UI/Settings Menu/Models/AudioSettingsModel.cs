public class AudioSettingsModel
{
    public const float DefaultMaster = 1f;
    public const float DefaultMusic = 1f;
    public const float DefaultSFX = 1f;
    public const float DefaultUI = 1f;

    public float Master;
    public float Music;
    public float SFX;
    public float UI;

    public static AudioSettingsModel CreateDefault() => new AudioSettingsModel
    {
        Master = DefaultMaster,
        Music = DefaultMusic,
        SFX = DefaultSFX,
        UI = DefaultUI
    };
    public AudioSettingsModel Clone() => new AudioSettingsModel
    {
        Master = Master,
        Music = Music,
        SFX = SFX,
        UI = UI
    };
}