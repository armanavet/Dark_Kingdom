using System;

[Serializable]
public class DisplaySettingsModel
{
    public int ResolutionIndex;
    public int ScreenModeIndex;
    public DisplaySettingsModel Clone()
    {
        return new DisplaySettingsModel
        {
            ResolutionIndex = ResolutionIndex,
            ScreenModeIndex = ScreenModeIndex
        };
    }
}
