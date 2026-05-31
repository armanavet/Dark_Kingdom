using System;
using UnityEngine;

public interface ISettingsRepository
{
    bool HasDisplaySave();
    bool HasQualitySave();
    bool HasAudioSave();
    bool HasControlsSave();
    DisplaySettingsModel LoadDisplay(int resIndex, int modeIndex);
    QualitySettingsModel LoadQuality(int qualityIndex);
    AudioSettingsModel LoadAudio();
    ControlsSettingsModel LoadControls();
    void SaveDisplay(DisplaySettingsModel display);
    void SaveQuality(QualitySettingsModel quality);
    void SaveAudio(AudioSettingsModel audio);
    void SaveControls(ControlsSettingsModel controls);
}
