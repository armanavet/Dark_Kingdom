using System;
using UnityEngine;

[Serializable]
public class AudioSettingsModel
{
    public float Master;
    public float Music;
    public float SFX;

    public AudioSettingsModel Clone()
    {
        return new AudioSettingsModel
        {
            Master = Master,
            Music = Music,
            SFX = SFX
        };
    }
}