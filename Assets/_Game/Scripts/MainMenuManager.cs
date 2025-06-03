using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class MainMenuManager : MonoBehaviour
{
    public string _SceneToLoad;
    public void a_BTStartGame()
    {
        SceneManager.LoadScene(_SceneToLoad);
    }

    public void b_BTExitButton()
    {
        Application.Quit();
    }

    //TODO Options button
}
