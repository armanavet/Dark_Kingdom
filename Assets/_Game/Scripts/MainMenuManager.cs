using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class MainMenuManager : MonoBehaviour
{
    public string _SceneToLoad;
    int id;

    public void a_BTStartGame()
    {
        SaveManager.SetSlot(id);
        SaveManager.OnGameStart();
        SceneManager.LoadScene(_SceneToLoad);
    }

    
    public void b_BTTest(int id)
    {
        this.id = id;
    }
    public void z_BTExitButton()
    {
        Application.Quit();
    }
    //F => savemetadata { }
    //TODO Options button
}
