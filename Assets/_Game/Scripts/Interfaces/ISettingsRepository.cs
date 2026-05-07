using UnityEngine;

public interface ISettingsRepository
{
    DisplaySettingsModel LoadDesplay(int resIndex, int modeIndex);
    AudioSettingsModel LoadAudio();
    ControlsSettingsModel LoadControls();
    void Save(DisplaySettingsModel display, AudioSettingsModel audio, ControlsSettingsModel controls);
}
