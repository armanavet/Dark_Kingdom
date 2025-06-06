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
    [SerializeField] List<SavePanelInfo> savePanelInfos;

    private void Start()
    {
        
        for (int i = 1; i <= savePanelInfos.Count; i++)
        {
            SaveMetaData data = SaveManager.LoadMetaData(i);
            savePanelInfos[i-1].test(data);

        }
    }
    public void a_BTStartGame()
    {
        SaveManager.SetSlot(id);
        SaveManager.OnGameStart();
        SceneManager.LoadScene(_SceneToLoad);
    }

    public void b_BTGetTheSaveSlotId(int id)
    {
        this.id = id;
    }
    public void z_BTExitButton()
    {
        Application.Quit();
    }
    public void test()
    {

    }
    //F => savemetadata { }
    //TODO Options button
}
