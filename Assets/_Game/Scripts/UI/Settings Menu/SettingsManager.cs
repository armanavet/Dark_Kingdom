using System;
using UnityEngine;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    [SerializeField] private SettingsInstaller installer;
    [SerializeField] private DisplayView displayView;
    [SerializeField] private QualityView qualityView;
    [SerializeField] private AudioView audioView;
    [SerializeField] private ControlsView controlsView;
    [SerializeField]
    private Button
        B_audio,
        B_video,
        B_controls,
        B_save,
        B_reset,
        B_confirm,
        B_deny,
        B_close;
    [SerializeField]
    private GameObject
        P_warning,
        P_buttons,//P_buttons refers to the Apply and Revert to Last Saved buttons.  
        P_audio,
        P_video,
        P_controls;
    private IViewModel[] viewModels;
    private ControlsViewModel controlsVM;

    private Action onApplyConfirmed;
    private Action onDenyConfirmed;

    private bool initialized;
    private void OnDisable()
    {
        if (controlsVM != null && controlsVM.IsRebinding)
            controlsVM.CancelRebind();
    }
    private void OnDestroy()
    {
        if (viewModels == null) return;

        foreach (var section in viewModels)
            section.OnChanged -= Refresh;
    }
    public void Initialize()
    {
                
        installer.Initialize();

        viewModels = installer.Sections;
        controlsVM = installer.ControlsVM;

        displayView.InitializeData(installer);
        audioView.InitializeData(installer);
        controlsView.InitializeData(installer);
        qualityView.InitializeData(installer);

        SetListeners();

        PanelsInitialState();

        foreach (var section in viewModels)
            section.OnChanged += Refresh;
        Refresh();
    }
    void RemoveAllListeners()
    {
        B_audio.onClick.RemoveAllListeners();
        B_video.onClick.RemoveAllListeners();
        B_controls.onClick.RemoveAllListeners();
        B_save.onClick.RemoveAllListeners();
        B_reset.onClick.RemoveAllListeners();
        B_confirm.onClick.RemoveAllListeners();
        B_deny.onClick.RemoveAllListeners();
        B_close.onClick.RemoveAllListeners();
    }
    void SetListeners()
    {
        RemoveAllListeners();   
        B_audio.onClick.AddListener(OnAudioClicked);
        B_video.onClick.AddListener(OnVideoClicked);
        B_controls.onClick.AddListener(OnControlsClicked);

        B_save.onClick.AddListener(OnSaveClicked);
        B_reset.onClick.AddListener(OnDefaultClicked);

        B_confirm.onClick.AddListener(OnWarningApply);
        B_deny.onClick.AddListener(OnWarnignDeny);

        B_close.onClick.AddListener(CloseWarning);
    }
    public void TryExit(Action onExitConfirmed)
    {
        if (AnyHasChanges())
        {
            ShowWarning(
                onApply: () => { ApplyAll(); onExitConfirmed?.Invoke(); },
                onDeny: () => { RevertAllToSaved(); onExitConfirmed?.Invoke(); }
            );
        }
        else
        {
            onExitConfirmed?.Invoke();
        }
    }
    void OnSaveClicked()
    {
        ShowWarning(
            onApply: ApplyAll,
            onDeny: RevertAllToSaved
            );
    }
    void OnDefaultClicked()
    {
        foreach (var viewModel in viewModels)
            viewModel.ResetToDefault();
    }
    bool AnyHasChanges()
    {
        foreach (var section in viewModels)
            if (section.HasChanges) return true;
        return false;
    }
    void ApplyAll()
    {
        foreach (var section in viewModels)
            section.Apply();
    }
    void RevertAllToSaved()
    {
        foreach (var section in viewModels)
            section?.RevertToSaved();
    }
    #region WARNING PANEL
    void ShowWarning(Action onApply, Action onDeny)
    {
        onApplyConfirmed = onApply;
        onDenyConfirmed = onDeny;
        P_warning?.SetActive(true);
    }
    void HideWarning()
    {
        P_warning?.SetActive(false);
        onApplyConfirmed = null;
        onDenyConfirmed = null;
    }
    void OnWarningApply()
    {
        onApplyConfirmed?.Invoke();
        HideWarning();
    }
    void OnWarnignDeny()
    {
        onDenyConfirmed?.Invoke();
        HideWarning();
    }
    void CloseWarning()
    {
        P_warning?.SetActive(false);
    }
    #endregion
    #region Panels Manage
    void PanelsInitialState()
    {
        P_audio?.SetActive(true);

        P_video?.SetActive(false);
        P_controls?.SetActive(false);
        P_warning?.SetActive(false);
    }
    void OnAudioClicked()
    {
        Debug.Log("opned");
        P_video.SetActive(false);
        P_controls.SetActive(false);
        P_audio.SetActive(true);
    }
    void OnVideoClicked()
    {
        P_audio.SetActive(false);
        P_controls.SetActive(false);
        P_video.SetActive(true);
    }
    void OnControlsClicked()
    {
        P_audio.SetActive(false);
        P_video.SetActive(false);
        P_controls.SetActive(true);
    }
    #endregion
    void Refresh()
    {
        B_save.gameObject.SetActive(AnyHasChanges());
    }
}
