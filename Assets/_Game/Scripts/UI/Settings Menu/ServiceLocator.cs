using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class ServiceLocator : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private InputActionAsset inputActions;
    [Range(1f, 2f)][SerializeField] private float minAspectRatioSize = 1.5f;
    [SerializeField] private QualityConfigRegistry configRegistry;

    public IDisplayService DisplayService { get; private set; }
    public IQualityService QualityService { get; private set; }
    public IAudioService AudioService { get; private set; }
    public IInputService InputService { get; private set; }
    public ISettingsRepository Repository { get; private set; }

    public void Initialize()
    {
        Repository = new PlayerPrefsSettingsRepository();
        DisplayService = new DisplayService(minAspectRatioSize);
        QualityService = new QualityService(configRegistry);
        AudioService = new AudioService(mixer);
        InputService = new ControlsService(inputActions);
    }
}
