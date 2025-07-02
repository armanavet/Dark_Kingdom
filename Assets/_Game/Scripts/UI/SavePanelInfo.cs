using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavePanelInfo : MonoBehaviour
{
    [SerializeField] TMP_Text PlayTimeText,GoldAmountText,CurrentWavetext,NewGameText;
    [SerializeField] GameObject SavedDataPanel;
    
    public void test(SaveMetaData saveMetaData)
    {
        if (saveMetaData == null)
        {
            SavedDataPanel.SetActive(false);
            NewGameText.gameObject.SetActive(true);
        }
        else
        {
            SavedDataPanel.SetActive(true);
            NewGameText.gameObject.SetActive(false);
            PlayTimeText.text = "Play Time = " + saveMetaData.PlayTime.ToString();
            GoldAmountText.text = "Current Gold = " + saveMetaData.CurrentGold.ToString();
            CurrentWavetext.text = "Current Wave = " + saveMetaData.CurrentWave.ToString();

        }
        

    }

}
