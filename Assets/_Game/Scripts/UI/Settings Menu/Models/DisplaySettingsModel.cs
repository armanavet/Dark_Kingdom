using System;

public class DisplaySettingsModel
{
    public int ResolutionIndex;
    public int ScreenModeIndex;

    public static DisplaySettingsModel CreateDefault(int resIndex, int modeIndex) => new DisplaySettingsModel
    {
        ResolutionIndex = resIndex,
        ScreenModeIndex = modeIndex
    };
    public DisplaySettingsModel Clone() => new DisplaySettingsModel
    {
        ResolutionIndex = ResolutionIndex,
        ScreenModeIndex = ScreenModeIndex
    };
}
