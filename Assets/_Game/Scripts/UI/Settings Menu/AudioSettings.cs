using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider master, music, sfx;
    [SerializeField] private TextMeshProUGUI masterText, musicText, sfxText;

    private SettingsViewModel viewModel;
    public void InitializeData(SettingsInstaller installer) 
    {
        viewModel = installer.ViewModel;

        master.onValueChanged.AddListener(OnMasterChanged);
        music.onValueChanged.AddListener(OnMusicChanged);
        sfx.onValueChanged.AddListener(OnSFXChanged);

        viewModel.OnChanged += Refresh;
        Refresh();
    }
    void OnMasterChanged(float value)
    {
        viewModel.SetMaster(value);
        UpdateLabel(masterText, value);
    }

    void OnMusicChanged(float value)
    {
        viewModel.SetMusic(value);
        UpdateLabel(musicText, value);
    }

    void OnSFXChanged(float value)
    {
        viewModel.SetSFX(value);
        UpdateLabel(sfxText, value);
    }

    void UpdateLabel(TextMeshProUGUI label, float value)
    {
        Debug.Log("enter");
        label.text = Mathf.RoundToInt(value * 100).ToString();
    }

    void Refresh()
    {
        master.SetValueWithoutNotify(viewModel.PendingAudioData.Master);
        music.SetValueWithoutNotify(viewModel.PendingAudioData.Music);
        sfx.SetValueWithoutNotify(viewModel.PendingAudioData.SFX);

        UpdateLabel(masterText, viewModel.PendingAudioData.Master);
        UpdateLabel(musicText, viewModel.PendingAudioData.Music);
        UpdateLabel(sfxText, viewModel.PendingAudioData.SFX);
    }
}
