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
        P_main.SetActive(true);
        P_start.SetActive(false);
        P_settings.SetActive(false);
        SettingsManager.Load();
    }
    public event Action OnBackFromSettings;
    void OnStartClikced()
    {
        //Close the Main Menu panel and open the start new game/load.
        P_main.SetActive(false);
        P_start.SetActive(true);
    }
    void OnSettingsClicked()
    {
        //Close the Main Menu panel and open the Settings panel.
        //SettingsManager.Instance.OnPlayerAction -= CloseSettingsMenu;
        P_main.SetActive(false);
        P_settings.SetActive(true);
    }
    void OnQuitClicked()
    {
        //Quit the game.
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
    }
    void OnSetSlotCkicked()
    {
        //Load the data and start the game.
    }

    void DetermineId()
    {
        //Determine the slot ID for save system.
    }
    void DetermineSceneToLoad()
    {
        //Determine the scene name where the main game takes place.
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

