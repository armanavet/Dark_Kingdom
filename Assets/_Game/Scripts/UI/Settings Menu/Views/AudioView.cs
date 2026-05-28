using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class AudioView: MonoBehaviour
{
    [SerializeField] private Slider master, music, sfx, ui;
    [SerializeField] private TextMeshProUGUI masterText, musicText, sfxText,uiText;

    private AudioViewModel viewModel;
    private void OnDestroy()
    {
        if (viewModel == null) return;
        viewModel.OnChanged -= Refresh;
    }
    public void InitializeData(SettingsInstaller installer) 
    {
        viewModel = installer.AudioVM;

        SetListeners();

        viewModel.OnChanged += Refresh;
        Refresh();
    }
    void RemoveAllListeners() 
    {
        master.onValueChanged.RemoveAllListeners();
        music.onValueChanged.RemoveAllListeners();
        sfx.onValueChanged.RemoveAllListeners();
        ui.onValueChanged.RemoveAllListeners();
    }
    void SetListeners() 
    {
        master.onValueChanged.AddListener(OnMasterChanged);
        music.onValueChanged.AddListener(OnMusicChanged);
        sfx.onValueChanged.AddListener(OnSFXChanged);
        ui.onValueChanged.AddListener(OnUIChanged);
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
    void OnUIChanged(float value)
    {
        viewModel.SetUI(value);
        UpdateLabel(sfxText, value);
    }
    void UpdateLabel(TextMeshProUGUI label, float value)
    {
        label.text = Mathf.RoundToInt(value * 100).ToString();
    }
    void Refresh()
    {
        master.SetValueWithoutNotify(viewModel.PendingData.Master);
        music.SetValueWithoutNotify(viewModel.PendingData.Music);
        sfx.SetValueWithoutNotify(viewModel.PendingData.SFX);
        ui.SetValueWithoutNotify(viewModel.PendingData.UI);

        UpdateLabel(masterText, viewModel.PendingData.Master);
        UpdateLabel(musicText, viewModel.PendingData.Music);
        UpdateLabel(sfxText, viewModel.PendingData.SFX);
        UpdateLabel(uiText, viewModel.PendingData.UI);
    }
}
