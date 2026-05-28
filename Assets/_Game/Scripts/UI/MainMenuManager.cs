using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using System;

public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// TO DO
    /// Manage panels conactions logic
    /// Manage player prefs save logic
    /// </summary>
    [SerializeField] private List<Button> saveSlots = new List<Button>();
    [SerializeField]
    private Button
        B_start,
        B_settings,
        B_quit,
        B_back_Start,
        B_back_Settings;
    [SerializeField]
    private GameObject
        P_main,
        P_start,
        P_settings;
    [SerializeField] private SettingsManager SettingsManager;
    private string sceneToLoad;
    private int id;
    #region Singleton 
    private static MainMenuManager _instance;
    public static MainMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MainMenuManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        Load();
    }
    #endregion
    void Load()
    {
        SettingsManager.Initialize();
        B_start.onClick.AddListener(OnStartClikced);
        B_settings.onClick.AddListener(OnSettingsClicked);
        B_quit.onClick.AddListener(OnQuitClicked);
        B_back_Start.onClick.AddListener(OnReturnFromStartGame);
        B_back_Settings.onClick.AddListener(OnReturnFromSettings);

        P_main.SetActive(true);
        P_start.SetActive(false);
        P_settings.SetActive(false);
    }
    void OnStartClikced()
    {
        P_main.SetActive(false);
        P_start.SetActive(true);
    }
    void OnSettingsClicked()
    {
        P_main.SetActive(false);
        P_settings.SetActive(true);
    }
    void OnQuitClicked()
    {
        Application.Quit();
    }
    void OnReturnFromStartGame()
    {
        P_main.SetActive(true);
        P_start.SetActive(false);
    }
    void OnReturnFromSettings()
    {
        SettingsManager.TryExit(CloseSettingsMenu);
    }
    public void CloseSettingsMenu()
    {
        P_main.SetActive(true);
        P_settings.SetActive(false);
    }
    void OnEmptySLotClicked()
    {
        //Save the data and start a new game
        //Recive a slot id. 
        //Save the id.
    }
    void OnSetSlotCkicked()
    {
        //Load the data and start the game.
        //Recive a slot id.
        //Load the id data.
    }


}

//------ Old logic:
//public string _SceneToLoad;
//int id;
//[SerializeField] List<SavePanelInfo> savePanelInfos;
//private void Start()
//{
//    for (int i = 1; i <= savePanelInfos.Count; i++)
//    {
//        SaveMetaData data = SaveManager.LoadMetaData(i);
//        savePanelInfos[i - 1].test(data);
//    }
//}
//public void a_BTStartGame()
//{

//    SaveManager.SetSlot(id);
//    SaveManager.OnGameStart();
//    SceneManager.LoadScene(_SceneToLoad);
//}

//public void b_BTGetTheSaveSlotId(int id)
//{
//    this.id = id;
//}
//public void z_BTExitButton()
//{
//    Application.Quit();
//}

