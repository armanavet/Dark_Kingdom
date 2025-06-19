using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public string _SceneToLoad;
    [SerializeField] private GameObject _PauseMenuContainer = null;
    bool isActive;
    float a;
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
        _PauseMenuContainer.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
    }
    public void z_BTExitToMenuButton()
    {
        SceneManager.LoadScene(_SceneToLoad);
    }
    //TODO Options button
}
