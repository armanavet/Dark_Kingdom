using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavePanelInfo : MonoBehaviour
{
    [SerializeField] TMP_Text PlayTimeText,GoldAmountText,CurrentWavetext,NewGameText;
    
    public void test(SaveMetaData saveMetaData)
    {
        if (saveMetaData == null)
        {
            PlayTimeText.gameObject.SetActive(false);
            GoldAmountText.gameObject.SetActive(false);
            CurrentWavetext.gameObject.SetActive(false);
            NewGameText.gameObject.SetActive(true);

        }
        else
        {
            PlayTimeText.gameObject.SetActive(true);
            GoldAmountText.gameObject.SetActive(true);
            CurrentWavetext.gameObject.SetActive(true);
            NewGameText.gameObject.SetActive(false);
            PlayTimeText.text = saveMetaData.PlayTime.ToString();
            GoldAmountText.text = saveMetaData.CurrentGold.ToString();
            CurrentWavetext.text = saveMetaData.CurrentWave.ToString();

        }
        

    }

}
