using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SettingsInstaller : MonoBehaviour
{

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private InputActionAsset inputActions;
    public SettingsViewModel ViewModel { get; private set; }
    public IDisplayService DisplayService { get; private set; }
    public IInputService InputService { get; private set; }

    private void Awake()
    {
        var repository = new PlayerPrefsSettingsRepository();
        DisplayService = new DisplayService();
        var audio = new AudioService(mixer);
        InputService = new ControlsService(inputActions);

        ViewModel = new SettingsViewModel(
            repository, 
            DisplayService, 
            audio,
            InputService);  
    }
}
