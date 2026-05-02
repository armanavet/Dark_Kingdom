using UnityEngine;
using UnityEngine.Audio;

public class AudioService : IAudioService
{
    private readonly AudioMixer mixer;
    public AudioService(AudioMixer mixer)
    {
        this.mixer = mixer;
    }
    float LinearToDB(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
    }
    public void Apply(AudioSettingsModel settings)
    {
        mixer.SetFloat(ConstValues.PREF_MASTER_VOLUME, LinearToDB(settings.Master));
        mixer.SetFloat(ConstValues.PREF_MUSIC_VOLUME, LinearToDB(settings.Music));
        mixer.SetFloat(ConstValues.PREF_SFX_VOLUME, LinearToDB(settings.SFX));

    }
}
