using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public string _SceneToLoad;
    [SerializeField] GameObject _PauseMenuPanel = null;
    //[SerializeField] GameObject _PauseMenuContainer = null;
    //bool isActive;
    //float a;
    #region Singleton 
    private static PauseMenuManager _instance;
    public static PauseMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PauseMenuManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    public void ShowPauseMenu(bool b)
    {
        _PauseMenuPanel.SetActive(b);
    }
    public void b_BTAudioPlay()
    {
        //AudioManager.Instance.PlayClickSoundForUI();
    }
    public void z_BTExitToMenuButton()
    {
        SceneManager.LoadScene(_SceneToLoad);
    }
    
}
