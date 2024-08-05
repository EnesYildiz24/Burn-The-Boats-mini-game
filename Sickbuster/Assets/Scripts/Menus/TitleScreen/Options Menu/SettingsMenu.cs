using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using Unity.Profiling;

public class SettingsMenu : MonoBehaviour
{
    // public Slider _musicSlider, _sfxSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;
    // AudioManager am;

    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
            
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);

            if(filteredResolutions[i].width == Screen.width && 
            filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // public void MusicVolume(){
    //     am.SetMusicVol(IngameScreenMusic.ingameMusicSource);
    //     TitleScreenMusic.Instance.SetMusicVol(_musicSlider.value);
    //     IngameScreenMusic.Instance.SetMusicVol(_musicSlider.value);
    // }
    // public void SFXVolume(){
    //     TitleScreenMusic.Instance.SetSoundVol(_sfxSlider.value);
    //     IngameScreenMusic.Instance.SetSoundVol(_sfxSlider.value);
    // }
}
