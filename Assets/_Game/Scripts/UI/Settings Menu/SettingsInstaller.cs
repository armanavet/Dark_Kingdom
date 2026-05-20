using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
public class SettingsInstaller : MonoBehaviour
{
    [SerializeField] private ServiceLocator locator;

    public IViewModel[] Sections { get; private set; }
    public DisplayViewModel DisplayVM { get; private set; }
    public QualityViewModel QualityVM { get; private set; }
    public AudioViewModel AudioVM { get; private set; }
    public ControlsViewModel ControlsVM { get; private set; }
    public void Initialize()
    {
        locator.Initialize();

        DisplayVM = new DisplayViewModel(locator.Repository, locator.DisplayService);
        QualityVM = new QualityViewModel(locator.Repository, locator.QualityService);
        AudioVM = new AudioViewModel(locator.Repository, locator.AudioService);
        ControlsVM = new ControlsViewModel(locator.Repository, locator.InputService);

        Sections = new IViewModel[]
        {
            DisplayVM,
            QualityVM,
            AudioVM,
            ControlsVM
        };
    }
}
