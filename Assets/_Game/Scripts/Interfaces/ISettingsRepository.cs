using UnityEngine;

public interface ISettingsRepository
{
    DisplaySettingsModel LoadDesplay(int resIndex, int modeIndex);
    AudioSettingsModel LoadAudio();
    void Save(DisplaySettingsModel display, AudioSettingsModel audio);
}
