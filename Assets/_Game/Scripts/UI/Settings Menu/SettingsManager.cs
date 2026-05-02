using System;
using UnityEngine;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    [SerializeField] private SettingsInstaller installer;
    [SerializeField] private DisplaySettings displaySettings;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField]
    private Button
        B_apply,
        B_reset,
        B_confirm,
        B_deny,
        B_close;
    [SerializeField] private GameObject P_warning, P_Buttons;//P_Buttons refers to the Apply and reset to Default buttons.  
    
    private SettingsViewModel vm;

    
    public event Action onClosedRequested;
    
    public void Load()
    {
        vm = installer.ViewModel;

        displaySettings.InitializeData(installer);
        audioSettings.InitializeData(installer);

        B_apply.onClick.AddListener(OnApply);
        B_reset.onClick.AddListener(OnDefault);

        B_confirm.onClick.AddListener(OnConfirm);
        B_deny.onClick.AddListener(OnDeny);

        B_close.onClick.AddListener(CloseWarning);

        vm.OnChanged += Refresh;
        Refresh();
    }
    public void TryExit(Action onExitConfirmed)
    {
        if (vm.HasChanges)
        {
            onClosedRequested = onExitConfirmed;
            OpenWarning();
        }
        else
        {
            onExitConfirmed?.Invoke();
        }
    }
    void OpenWarning()
    {
        P_Buttons.SetActive(false);
        P_warning.SetActive(true);
    }
    void CloseWarning()
    {
        P_warning.SetActive(false);
        P_Buttons.SetActive(true);
    }
    void OnApply()
    {
        vm.Apply();
        P_Buttons.SetActive(false);
    }
    void OnDefault()
    {
        vm.ResetToDefault();
        P_Buttons.SetActive(false);
    }
    void OnConfirm()
    {
        vm.Apply();
        ExitSettings();
    }
    void OnDeny()
    {
        vm.DenyChanges();
        ExitSettings();
    }
    void ExitSettings()
    {
        CloseWarning();
        onClosedRequested?.Invoke();
    }

    void Refresh()
    {
        //P_Buttons.gameObject.SetActive(vm.HasChanges);
        P_Buttons.gameObject.SetActive(true);
    }
  
}
