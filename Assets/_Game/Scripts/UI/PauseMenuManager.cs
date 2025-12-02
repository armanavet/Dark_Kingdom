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
    bool isActive;
    float a;
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
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            a_BTPause();
        }
        // else => continue all actions
    }
    public void a_BTPause()
    {
        isActive = !isActive;
        _PauseMenuPanel.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
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
