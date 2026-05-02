using UnityEngine;
using UnityEngine.Audio;

public class SettingsInstaller : MonoBehaviour
{
    public SettingsViewModel ViewModel { get; private set; }
    public IDisplayService DisplayService { get; private set; }
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        var repository = new PlayerPrefsSettingsRepository();
        DisplayService = new DisplayService();
        var audio = new AudioService(mixer);

        ViewModel = new SettingsViewModel(repository, DisplayService, audio);  
    }
}
