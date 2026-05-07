using UnityEngine;

public class PlayerPrefsSettingsRepository : ISettingsRepository
{
    static string res = ConstValues.PREF_RESOLUTION_INDEX;
    static string mode = ConstValues.PREF_SCREEN_MODE_INDEX;
    static string master = ConstValues.PREF_MASTER_VOLUME;
    static string music = ConstValues.PREF_MUSIC_VOLUME;
    static string sfx = ConstValues.PREF_SFX_VOLUME;

    static string rebinds = ConstValues.PREF_REBINDS;
    static string moveSpeed = ConstValues.PREF_MOVE_SPEED;
    static string rotationSpeed = ConstValues.PREF_ROTATION_SPEED;
    static string zoomSpeed = ConstValues.PREF_ZOOM_SPEED;
    static string mouseDragSpeed = ConstValues.PREF_MOUSE_DRAG_SPEED;


    static float master_value = ConstValues.DEFAULT_MASTER_VOLUME;
    static float music_value = ConstValues.DEFAULT_MUSIC_VOLUME;
    static float sfx_value = ConstValues.DEFAULT_SFX_VOLUME;

    public DisplaySettingsModel LoadDesplay(int resIndex, int modeIndex)
    {
        return new DisplaySettingsModel
        {
            ResolutionIndex = PlayerPrefs.GetInt(res, resIndex),
            ScreenModeIndex = PlayerPrefs.GetInt(mode, modeIndex)
        };
    }

    public AudioSettingsModel LoadAudio()
    {
        return new AudioSettingsModel
        {
            Master = PlayerPrefs.GetFloat(master, master_value),
            Music = PlayerPrefs.GetFloat(music, music_value),
            SFX = PlayerPrefs.GetFloat(sfx, sfx_value)
        };
    }

    public ControlsSettingsModel LoadControls()
    {
        return new ControlsSettingsModel
        {
            RebindJson = PlayerPrefs.GetString(rebinds, string.Empty),
            MovementSpeed = PlayerPrefs.GetFloat(moveSpeed, 1f),
            RotationSpeed = PlayerPrefs.GetFloat(rotationSpeed, 1f),
            ZoomSpeed = PlayerPrefs.GetFloat(zoomSpeed, 1f),
            DragSpeed = PlayerPrefs.GetFloat(mouseDragSpeed, 1f)
        };
    }
    public void Save(DisplaySettingsModel display, AudioSettingsModel auido, ControlsSettingsModel controls)
    {
        PlayerPrefs.SetInt(res, display.ResolutionIndex);
        PlayerPrefs.SetInt(mode, display.ScreenModeIndex);

        PlayerPrefs.SetFloat(master, auido.Master);
        PlayerPrefs.SetFloat(music, auido.Music);
        PlayerPrefs.SetFloat(sfx, auido.SFX);

        PlayerPrefs.SetString(rebinds, controls.RebindJson);
        PlayerPrefs.SetFloat(moveSpeed, controls.MovementSpeed);
        PlayerPrefs.SetFloat(rotationSpeed, controls.RotationSpeed);
        PlayerPrefs.SetFloat(zoomSpeed, controls.ZoomSpeed);
        PlayerPrefs.SetFloat(mouseDragSpeed, controls.DragSpeed);

        PlayerPrefs.Save();
    }
}
