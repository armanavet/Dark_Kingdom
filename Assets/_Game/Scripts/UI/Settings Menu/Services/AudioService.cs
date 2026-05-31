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
        mixer.SetFloat(ConstValues.MIXER_MASTER, LinearToDB(settings.Master));
        mixer.SetFloat(ConstValues.MIXER_MUSIC, LinearToDB(settings.Music));
        mixer.SetFloat(ConstValues.MIXER_SFX, LinearToDB(settings.SFX));
        mixer.SetFloat(ConstValues.MIXER_UI, LinearToDB(settings.UI));
    }
}
