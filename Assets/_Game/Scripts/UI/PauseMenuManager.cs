using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AudioSystem;
using System;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private string SceneToLoad;
    [SerializeField]
    private Button
        B_continue,
        B_settings,
        B_exit,
        B_confirm,
        B_deny,
        B_back_Settings;
    [SerializeField] private GameObject P_pauseMenu, P_mainPanel, P_settings, P_exitWarning;
    [SerializeField] private SettingsManager settingsManager;
    SoundData soundData;
    #region Singleton 
    private static PauseMenuManager _instance;
    public static PauseMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PauseMenuManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public void Initialize()
    {
        soundData = AudioManager.Instance.GetData(SoundDataType.UI);
        settingsManager.Initialize();
        SetButtonsActions();
        SetPanelsConditions();
    }
    void RemoveAllListeners()
    {
        B_continue.onClick.RemoveAllListeners();
        B_settings.onClick.RemoveAllListeners();
        B_exit.onClick.RemoveAllListeners();
        B_confirm.onClick.RemoveAllListeners();
        B_deny.onClick.RemoveAllListeners();
        B_back_Settings.onClick.RemoveAllListeners();
    }
    void SetButtonsActions()
    {
        RemoveAllListeners();
        B_continue.onClick.AddListener(OnContinueClikced);
        B_settings.onClick.AddListener(OnSettingsClicked);
        B_exit.onClick.AddListener(OnExitClicked);
        B_confirm.onClick.AddListener(OnConfirm);
        B_deny.onClick.AddListener(OnDeny);
        B_back_Settings.onClick.AddListener(OnReturnFromSettings);
    }
    void SetPanelsConditions()
    {
        P_pauseMenu?.SetActive(false);
        P_mainPanel?.SetActive(true);
        P_settings?.SetActive(false);
        P_exitWarning?.SetActive(false);
    }
    void OnContinueClikced()
    {
        PlayAudio();
        P_pauseMenu.SetActive(false);
    }
    void OnSettingsClicked()
    {
        PlayAudio();
        P_mainPanel.SetActive(false);
        P_settings.SetActive(true);
    }
    void OnReturnFromSettings()
    {
        Debug.Log("");
        PlayAudio();
        settingsManager.TryExit(CloseSettingsPanel);
    }
    void CloseSettingsPanel()
    {
        P_settings.SetActive(false);
        P_mainPanel.SetActive(true);
    }
    void OnExitClicked()
    {
        PlayAudio();
        ShowWarning();
    }

    void ShowWarning()
    {
        P_exitWarning.SetActive(true);
    }
    void HideWarning()
    {
        P_exitWarning?.SetActive(false);
    }
    void OnConfirm()
    {
        StartCoroutine(WaitBeforeExit());
    }
    void OnDeny()
    {
        PlayAudio();
        HideWarning();
    }
    void PlayAudio()
    {
        AudioManager.Instance.Play(UISFX_Type.MouseLeftButton, soundData);
    }
    void LoadScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
    public void PauseMenuState(bool state)
    {
        PlayAudio();
        P_pauseMenu.SetActive(state);
    }

    IEnumerator WaitBeforeExit()
    {
        PlayAudio();
        yield return new WaitForSeconds(soundData.clip.length);
        LoadScene();
    }
}
