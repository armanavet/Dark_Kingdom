using UnityEngine;

public class PlayerPrefsSettingsRepository : ISettingsRepository
{
    static string res = ConstValues.PREF_RESOLUTION_INDEX;
    static string mode = ConstValues.PREF_SCREEN_MODE_INDEX;
    static string master = ConstValues.PREF_MASTER_VOLUME;
    static string music = ConstValues.PREF_MUSIC_VOLUME;
    static string sfx = ConstValues.PREF_SFX_VOLUME;

    static float maste_value = ConstValues.DEFAULT_MASTER_VOLUME;
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
            Music = PlayerPrefs.GetFloat(master, maste_value),
            Master = PlayerPrefs.GetFloat(music, music_value),
            SFX = PlayerPrefs.GetFloat(sfx, sfx_value)
        };
    }
    public void Save(DisplaySettingsModel display, AudioSettingsModel auido)
    {
        PlayerPrefs.SetInt(res, display.ResolutionIndex);
        PlayerPrefs.SetInt(mode, display.ScreenModeIndex);

        PlayerPrefs.SetFloat(master, auido.Master);
        PlayerPrefs.SetFloat(music, auido.Music);
        PlayerPrefs.SetFloat(sfx, auido.SFX);

        PlayerPrefs.Save();
    }
}
