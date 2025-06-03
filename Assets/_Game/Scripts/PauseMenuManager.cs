using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public string _SceneToLoad;
    [SerializeField] private GameObject _PauseMenuContainer = null;
    
    void Update()
    {
        
        if (Input.GetKeyDown("escape"))
        {
            _PauseMenuContainer.SetActive(true);
            //TODO Pause all actionsin game
        }
        // else => continue all actions
    }

    
    public void b_BTExitToMenuButton()
    {
        SceneManager.LoadScene(_SceneToLoad);
    }

    //TODO Options button
}
