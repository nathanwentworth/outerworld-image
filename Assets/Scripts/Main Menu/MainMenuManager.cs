﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenuManager : MonoBehaviour
{

    public Text LoadingText;
    public Slider ProgressBar;
    public GameObject LoadingContainer;
    public GameObject OptionsContainer;
    public GameObject MainContainer;
    private AsyncOperation sync;
    private bool startAnimation;

    public Slider options_VolumeSlider;
    public Dropdown options_ResolutionDrop;
    public Dropdown options_FullOrWindDrop;
    public Slider options_MouseSensitivity;
    public Text options_VolumeSliderValue;
    public Text options_MouseSliderValue;

    private string VOLUMEKEY = "VOLUME_VALUE";
    private string RESOLUTIONKEY = "RESOLUTION_VALUE";
    private string FULLSCREENKEY = "FULLSCREEN_VALUE";
    private string MOUSESENSITIVITYKEY = "MOUSESENSITIVITY_KEY";

    public float timeBetween;

    private bool wasResolutionChanged = false;
    private bool wasFullscreenChanged = false;


    void Start()
    {
        if (System.IO.Directory.Exists(Application.persistentDataPath + "/Photos/") != true)
        {
            Debug.Log("Creating Photos Directory");
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Photos/");
        }

        ProgressBar.value = 0;
        Time.timeScale = 1;
        LoadingContainer.SetActive(false);
        OptionsContainer.SetActive(false);
        MainContainer.SetActive(true);

        options_ResolutionDrop.options.Clear();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            options_ResolutionDrop.options.Add(new Dropdown.OptionData(Screen.resolutions[i].ToString()));
        }
        options_VolumeSlider.value = PlayerPrefs.GetFloat(VOLUMEKEY, 0.75f);
        options_FullOrWindDrop.value = PlayerPrefs.GetInt(FULLSCREENKEY, 0);
        options_ResolutionDrop.value = PlayerPrefs.GetInt(RESOLUTIONKEY, 0);
        options_MouseSensitivity.value = PlayerPrefs.GetFloat(MOUSESENSITIVITYKEY, 2.5f);

        options_ResolutionDrop.RefreshShownValue();

        if (options_FullOrWindDrop.value == 0)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    void Update()
    {
        if (startAnimation)
        {
            ProgressBar.value = Mathf.Lerp(ProgressBar.value, sync.progress + 0.1f, timeBetween);
        }

        if (ProgressBar.value >= 0.95f && sync.progress == 0.9f)
        {
            sync.allowSceneActivation = true;
        }

        // options_MouseSliderValue.text = string.Format("{0:F1}", options_MouseSensitivity.value);

        // options_VolumeSliderValue.text = string.Format("{0:F0}%", options_VolumeSlider.value * 100);
    }

    public void FreePlayButton()
    {
        StartCoroutine(LoadingScreen("Test"));
    }

    public void ScavHuntButton()
    {
        StartCoroutine(LoadingScreen("scav"));
    }

    public void TutorialButton()
    {

    }

    public void GalleryButton()
    {
        StartCoroutine(LoadingScreen("GalleryTest"));
    }

    public void OptionsButton()
    {
        MainContainer.SetActive(false);
        OptionsContainer.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ResolutionChanged()
    {
        wasResolutionChanged = true;
    }

    public void FullscreenChanged()
    {
        wasFullscreenChanged = true;
    }

    public void Options_Save()
    {
        PlayerPrefs.SetInt(FULLSCREENKEY, options_FullOrWindDrop.value);
        PlayerPrefs.SetFloat(MOUSESENSITIVITYKEY, options_MouseSensitivity.value);
        PlayerPrefs.SetInt(RESOLUTIONKEY, options_ResolutionDrop.value);
        PlayerPrefs.SetFloat(VOLUMEKEY, options_VolumeSlider.value);

        bool fullscreen = Screen.fullScreen;

        if (wasFullscreenChanged)
        {

            if (options_FullOrWindDrop.value == 0)
            {
                Screen.fullScreen = true;
                fullscreen = true;

            }
            else
            {
                Screen.fullScreen = false;
                fullscreen = false;
            }
        }

        if (wasResolutionChanged)
        {
            Screen.SetResolution(Screen.resolutions[options_ResolutionDrop.value].width, Screen.resolutions[options_ResolutionDrop.value].height, fullscreen, Screen.resolutions[options_ResolutionDrop.value].refreshRate);
            Debug.Log(Screen.resolutions[options_ResolutionDrop.value]);
        }

        Debug.Log(Screen.fullScreen);

        MainContainer.SetActive(true);
        OptionsContainer.SetActive(false);
        wasFullscreenChanged = false;
        wasResolutionChanged = false;
    }

    IEnumerator LoadingScreen(string whatScene)
    {
        LoadingContainer.SetActive(true);
        sync = SceneManager.LoadSceneAsync(whatScene, LoadSceneMode.Single);
        sync.allowSceneActivation = false;
        startAnimation = true;
        while (sync.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

}
