using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Setting : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle toggle;
    private  float currentVolume;
    private Resolution[] resolutions;
    
    private List<Resolution> targetResolutions;

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        targetResolutions = new List<Resolution>();
        int currentResolutionIndex = 0;
        GameManager gInst = GameManager.instance;

        Resolution tmpResol;
        bool isTarget;
        int j = 0;
        //j is for currentResolutionIndex 
        for (int i = 0 ; i < resolutions.Length; i++)
        {
            tmpResol = resolutions[i];
            isTarget = 
            GameManager.NearlyEqual(gInst.targetScreenRatio, (float)tmpResol.width / (float)tmpResol.height);
            //if tmpResol is not same to our targetResolution, move on to next resolution
            if( !isTarget )
                continue;
                
            
            string option = tmpResol.width + " x " + tmpResol.height;
            options.Add(option);
            targetResolutions.Add(tmpResol);

            if (tmpResol.width == Screen.currentResolution.width && tmpResol.height == Screen.currentResolution.height)
                currentResolutionIndex = j++;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetVolume(float volume)
    {
        volume = volumeSlider.value;
        audioMixer.SetFloat("Volume", volume);
        currentVolume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        isFullscreen = toggle.isOn;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        resolutionIndex = resolutionDropdown.value;
        Resolution resolution = targetResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("VolumePreference", currentVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings(int currentResolutionIndex)
    {

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;

        if (PlayerPrefs.HasKey("VolumePreference"))
            volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
        else
            volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
    }
}
