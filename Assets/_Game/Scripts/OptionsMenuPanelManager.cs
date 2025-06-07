using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenuPanelManager : MonoBehaviour
{
    [SerializeField] RectTransform UnderLine;
    //[SerializeField] Button AudioBT,VideoBT,GamePlayBT;
    [Header("Volume Setting")]
    [SerializeField] TMP_Text volumeTextValue = null;
    [SerializeField] Slider volumeSlider = null;
    [SerializeField] GameObject confirmationPrompt = null;
    [SerializeField] float defaultVolume = 0.5f;
    float savedVolume;
    private void Start()
    {
        savedVolume = PlayerPrefs.GetFloat("masterVolume", AudioListener.volume);
    }
    public void a_BTAddUnderline(int underlineXPos)
    {
        UnderLine.position = new Vector2(underlineXPos, UnderLine.position.y);

    }
    public void b_BTChangeUnderlineSize(int width)
    {
        UnderLine.sizeDelta = new Vector2(width, UnderLine.sizeDelta.y);
    }
    public void c_BTSetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }
    public void e_BTResetButton(string MenuType)
    {
        if(MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            f_BTVolumeApply();
        }
    }
    public void f_BTVolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        savedVolume = AudioListener.volume;
        StartCoroutine(ConfirmationBox());
    }
    public void g_BTBackButton()
    {
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");
    }
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
