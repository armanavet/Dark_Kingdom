using UnityEngine;

public class PlayerPrefsSettingsRepository : ISettingsRepository
{
    private const string KeyResolution = ConstValues.PREF_RESOLUTION_INDEX;
    private const string KeyScreenMode = ConstValues.PREF_SCREEN_MODE_INDEX;
    private const string KeyMaster = ConstValues.PREF_MASTER_VOLUME;
    private const string KeyMusic = ConstValues.PREF_MUSIC_VOLUME;
    private const string KeySFX = ConstValues.PREF_SFX_VOLUME;
    private const string KeyUI = ConstValues.PREF_UI_VOLUME;
    private const string KeyRebinds = ConstValues.PREF_REBINDS;
    private const string KeyMoveSpeed = ConstValues.PREF_MOVE_SPEED;
    private const string KeyRotation = ConstValues.PREF_ROTATION_SPEED;
    private const string KeyZoom = ConstValues.PREF_ZOOM_SPEED;
    private const string KeyDrag = ConstValues.PREF_MOUSE_DRAG_SPEED;
    private const string KeyQuality = ConstValues.PREF_QUALITY_INDEX;

    public bool HasDisplaySave() => PlayerPrefs.HasKey(KeyResolution);
    public bool HasQualitySave() => PlayerPrefs.HasKey(KeyQuality);
    public bool HasAudioSave() => PlayerPrefs.HasKey(KeyMaster);
    public bool HasControlsSave() => PlayerPrefs.HasKey(KeyRebinds);

    public DisplaySettingsModel LoadDisplay(int defaultResIndex, int defaultModeIndex)
    {
        return new DisplaySettingsModel
        {
            ResolutionIndex = PlayerPrefs.GetInt(KeyResolution, defaultResIndex),
            ScreenModeIndex = PlayerPrefs.GetInt(KeyScreenMode, defaultModeIndex)
        };
    }
    public QualitySettingsModel LoadQuality(int qualityIndex)
    {
        return new QualitySettingsModel
        {
            QualityIndex = PlayerPrefs.GetInt(KeyQuality, qualityIndex)
        };
    }
    public AudioSettingsModel LoadAudio()
    {
        return new AudioSettingsModel
        {
            Master = PlayerPrefs.GetFloat(KeyMaster, AudioSettingsModel.DefaultMaster),
            Music = PlayerPrefs.GetFloat(KeyMusic, AudioSettingsModel.DefaultMusic),
            SFX = PlayerPrefs.GetFloat(KeySFX, AudioSettingsModel.DefaultSFX),
            UI = PlayerPrefs.GetFloat(KeyUI, AudioSettingsModel.DefaultUI)
        };
    }
    public ControlsSettingsModel LoadControls()
    {
        return new ControlsSettingsModel
        {
            RebindJson = PlayerPrefs.GetString(KeyRebinds, string.Empty),
            MovementSpeed = PlayerPrefs.GetFloat(KeyMoveSpeed, ControlsSettingsModel.DefaultSpeed),
            RotationSpeed = PlayerPrefs.GetFloat(KeyRotation, ControlsSettingsModel.DefaultSpeed),
            ZoomSpeed = PlayerPrefs.GetFloat(KeyZoom, ControlsSettingsModel.DefaultSpeed),
            DragSpeed = PlayerPrefs.GetFloat(KeyDrag, ControlsSettingsModel.DefaultSpeed)
        };
    }
    public void SaveDisplay(DisplaySettingsModel display)
    {
        PlayerPrefs.SetInt(KeyResolution, display.ResolutionIndex);
        PlayerPrefs.SetInt(KeyScreenMode, display.ScreenModeIndex);

        PlayerPrefs.Save();
    }
    public void SaveQuality(QualitySettingsModel quality)
    {
        PlayerPrefs.SetInt(KeyQuality, quality.QualityIndex);

        PlayerPrefs.Save();
    }
    public void SaveAudio(AudioSettingsModel audio)
    {
        PlayerPrefs.SetFloat(KeyMaster, audio.Master);
        PlayerPrefs.SetFloat(KeyMusic, audio.Music);
        PlayerPrefs.SetFloat(KeySFX, audio.SFX);
        PlayerPrefs.SetFloat(KeyUI, audio.UI);

        PlayerPrefs.Save();
    }
    public void SaveControls(ControlsSettingsModel controls)
    {
        PlayerPrefs.SetString(KeyRebinds, controls.RebindJson);
        PlayerPrefs.SetFloat(KeyMoveSpeed, controls.MovementSpeed);
        PlayerPrefs.SetFloat(KeyRotation, controls.RotationSpeed);
        PlayerPrefs.SetFloat(KeyZoom, controls.ZoomSpeed);
        PlayerPrefs.SetFloat(KeyDrag, controls.DragSpeed);

        PlayerPrefs.Save();
    }

}
