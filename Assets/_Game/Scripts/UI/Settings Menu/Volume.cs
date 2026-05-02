using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Timeline;

public class Volume : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider, musicSlider, soundEffectsSlider;
    [SerializeField] private TextMeshProUGUI masterText, musicText, soundEffectsText;
    private const string MASTER = "Master";
    private const string MUSIC = "Music";
    private const string SFX = "SoundEffects";
    private const float DEFAULT_MASTER = 1f;
    private const float DEFAULT_MUSIC = 1f;
    private const float DEFAULT_SFX = 1f;

    //TO DO
    //Make save and reset system.
    void Start()
    {
        LoadVolume();
    }

    float LinearToDB(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
    }
    void ApplyVolume(string parameter, float value, TextMeshProUGUI label)
    {
        label.text = ((int)(value * 100)).ToString();
        mixer.SetFloat(parameter, LinearToDB(value));
    }
    void SetVolume(string parameter, float value, TextMeshProUGUI label, string prefKey)
    {
        ApplyVolume(parameter, value, label);
        PlayerPrefs.SetFloat(prefKey, value);
        PlayerPrefs.Save();
    }
    public void OnMasterVolumeChange()
    {
        SetVolume(MASTER, masterSlider.value, masterText, "MasterVolume");
    }
    public void OnMusicVolumeChange()
    {
        SetVolume(MUSIC, musicSlider.value, musicText, "MusicVolume");
    }
    public void OnSoundEffectsVolumeChange()
    {
        SetVolume(SFX, soundEffectsSlider.value, soundEffectsText, "SoundEffectsVolume");
    }
    void LoadVolume()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", DEFAULT_MASTER);
        float music = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_MUSIC);
        float sfx = PlayerPrefs.GetFloat("SoundEffectsVolume", DEFAULT_SFX);

        masterSlider.value = master;
        musicSlider.value = music;
        soundEffectsSlider.value = sfx;

        ApplyVolume(MASTER, master, masterText);
        ApplyVolume(MUSIC, music, musicText);
        ApplyVolume(SFX, sfx, soundEffectsText);
    }
    //Improvem manage with Settings manager.
    //public void ResetToDefaults()
    //{
    //    SetVolume(MASTER, DEFAULT_MASTER, masterText, "MasterVolume");
    //    SetVolume(MUSIC, DEFAULT_MUSIC, musicText, "MusicVolume");
    //    SetVolume(SFX, DEFAULT_SFX, soundEffectsText, "SoundEffectsVolume");

    //    masterSlider.value = DEFAULT_MASTER;
    //    musicSlider.value = DEFAULT_MUSIC;
    //    soundEffectsSlider.value = DEFAULT_SFX;
    //}
}
