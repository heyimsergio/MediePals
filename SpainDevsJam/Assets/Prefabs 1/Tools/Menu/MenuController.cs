using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour{

    [SerializeField] AudioMixer mainMixer = default;
    [Tooltip("The music volume the first time you start the game")] [SerializeField, Range(0, 1)] float defaultMusicValue = 1f;
    [Tooltip("The SFX volume the first time you start the game")] [SerializeField, Range(0, 1)] float defaultSfxValue = 1f;
    [SerializeField] Slider musicSlider = default;
    [SerializeField] Slider sfxSlider = default;

    [Space]
    [SerializeField] TMP_Dropdown qualityDropdown = default;
    int qualitySelected;

    [Space]
    [SerializeField] TMP_Dropdown resolutionDropdown = default;
    Resolution[] resolutions;
    int currentResolutionIndex;


    [Space]
    [SerializeField] Toggle fullScreen = default;

    bool musicVolumeSaved;

    void Awake(){

        if (resolutionDropdown != null){
            resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++){
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                    currentResolutionIndex = i;
                }
            }

            resolutions.Reverse();
            resolutionDropdown.AddOptions(options);

            if (PlayerPrefs.HasKey("ResolutionSelected"))
            {
                resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionSelected");
                currentResolutionIndex = resolutionDropdown.value;
            }
            else
            {
                resolutionDropdown.value = currentResolutionIndex;
            }

            resolutionDropdown.RefreshShownValue();
        }

        if(qualityDropdown != null){
            if (PlayerPrefs.HasKey("QualitySelected")){
                qualitySelected = PlayerPrefs.GetInt("QualitySelected");
            }else{
                qualitySelected = qualityDropdown.value;
            }

            qualityDropdown.value = qualitySelected;
            QualitySettings.SetQualityLevel(qualitySelected);
        }

        if(fullScreen != null)
        {
            if (!PlayerPrefs.HasKey("IsFullScreen"))
            {
                PlayerPrefs.SetInt("IsFullScreen", Screen.fullScreen ? 1 : 0);
            }
            if(PlayerPrefs.GetInt("IsFullScreen") == 1)
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
                Screen.fullScreen = false;

            fullScreen.GetComponent<Toggle>().isOn = Screen.fullScreen;
        }

    }


    void Start(){
        if (musicSlider != null && sfxSlider != null){
            if (!PlayerPrefs.HasKey("MusicVolume"))
                PlayerPrefs.SetFloat("MusicVolume", defaultMusicValue);


            if (!PlayerPrefs.HasKey("SFXVolume"))
                PlayerPrefs.SetFloat("SFXVolume", defaultSfxValue);

            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

            musicVolumeSaved = true;

            SetMusicVolume(musicSlider.value);
            SetSfxVolume(sfxSlider.value);
        }
    }

    //Needs a slider between 0.0001 and 1
    public void SetMusicVolume(float sliderValue){
        if (musicVolumeSaved)
        {
            mainMixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        }
    }

    //Needs a slider between 0.0001 and 1
    public void SetSfxVolume(float sliderValue){
        if (musicVolumeSaved)
        {
            mainMixer.SetFloat("SFX", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        }
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualitySelected", qualityIndex);
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionSelected", resolutionIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);

        if(Screen.fullScreen)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void Play(){

    }

    public void Quit(){
        Application.Quit();
    }
}
