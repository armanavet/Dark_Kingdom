using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

public class OptionsMenuPanelManager : MonoBehaviour
{
    [SerializeField] RectTransform UnderLine;
    //[SerializeField] Button AudioBT,VideoBT,GamePlayBT;
    [Header("Volume Setting")]
    [SerializeField] TMP_Text volumeTextValue = null;
    [SerializeField] Slider volumeSlider = null;
    [SerializeField] GameObject confirmationPrompt = null;
    [SerializeField] float defaultVolume = 0.5f;
    

    [Header("Graphic Setting")]
    [SerializeField] Slider brightnessSlider = null;
    [SerializeField] TMP_Text brightnessTextVolume = null;
    [SerializeField] float defaultBrightness = 1f;

    [Space(10)]
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    private float savedBrightness;
    private int savedQuality;
    private bool savedFullscreen;
    private int savedResolutionIndex;

    [Header("Resolution Dropdowns")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;


    float savedVolume;
    private void Start()
    {
        savedVolume = PlayerPrefs.GetFloat("masterVolume", AudioListener.volume);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        //---------------
        savedResolutionIndex = currentResolutionIndex;
        savedBrightness = defaultBrightness;
        resolutionDropdown.RefreshShownValue();
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

        if(MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextVolume.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height,Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            
            k_BTGraphicsApply();
        }
    }
    public void f_BTVolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        savedVolume = AudioListener.volume;
        StartCoroutine(ConfirmationBox());
    }
    public void g_BTBackButtonVolume()
    {
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");
    }
    public void h_BTSetBrightness(float brightness) 
    {
        _brightnessLevel = brightness;
        brightnessTextVolume.text = brightness.ToString("0.0");
    }

    public void i_BTSetFullScreen(bool isFullscreen) 
    {
        _isFullScreen = isFullscreen;
    }

    public void j_BTSetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }
    public void k_BTGraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        
        QualitySettings.SetQualityLevel(_qualityLevel);
        Screen.fullScreen = _isFullScreen;

        savedBrightness = _brightnessLevel;
        savedQuality = _qualityLevel;
        savedFullscreen = _isFullScreen;
        savedResolutionIndex = resolutionDropdown.value;
        


        StartCoroutine(ConfirmationBox());
    }
    public void l_BTSetResolution(int resolutionIndex) 
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,Screen.fullScreen);
    }
    public void m_BTBackButtonGraphics()
    {
        _brightnessLevel = savedBrightness;
        _qualityLevel = savedQuality;
        _isFullScreen = savedFullscreen;

        PlayerPrefs.SetFloat("masterBrightness", savedBrightness);
        brightnessSlider.value = savedBrightness;
        brightnessTextVolume.text = savedBrightness.ToString("0.0");
        
        qualityDropdown.value = savedQuality;
        QualitySettings.SetQualityLevel(savedQuality);

        fullScreenToggle.isOn = savedFullscreen;
        Screen.fullScreen = savedFullscreen;

        resolutionDropdown.value = savedResolutionIndex;
        Resolution resolution = resolutions[savedResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, savedFullscreen);

        
    }
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
