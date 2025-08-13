using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Main_Menu : MonoBehaviour
{
    public GameObject ContinueButton;

    [Header("Audio Settings")]
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private Slider MusicSlider = null;

    [Header("Graphics Settings")]
    [SerializeField] private Toggle FullscreenToggle = null;

    private bool _isFullScreen;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropDown; 
    private Resolution[] resolutions;

    private string levelToLoad;

    private void Awake()
    {
        LoadPlayerPrefs();
    }
    void Start()
    {
        //check for save game
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedGame");
            SceneManager.LoadScene(levelToLoad);
            ContinueButton.SetActive(true);
        }
        else
        {
            ContinueButton.SetActive(false);
        }

        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResoltionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResoltionIndex = i;
            }
        
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResoltionIndex;
        resolutionDropDown.RefreshShownValue();

        if (PlayerPrefs.HasKey("MasterResolutionIndex"))
        {
            Resolution resolution = resolutions[PlayerPrefs.GetInt("MasterResolutionIndex")];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            resolutionDropDown.SetValueWithoutNotify(PlayerPrefs.GetInt("MasterResolutionIndex"));
        }
    }

    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume"));

        }

        if (PlayerPrefs.HasKey("MasterFullscreen"))
        {
            int fullScreenValue = PlayerPrefs.GetInt("MasterFullscreen");
            if (fullScreenValue == 1)
            {
                _isFullScreen = true;
            }
            else
            {
                _isFullScreen = false;
            }
            Screen.fullScreen = _isFullScreen;
            FullscreenToggle.SetIsOnWithoutNotify(_isFullScreen);
        }
        

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("MasterResolutionIndex", resolutionIndex);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Pre_Game");
    }

    public void ContinueGame()
    {
        //do something to do later
        SceneManager.LoadSceneAsync(levelToLoad);
    }

    public void setVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
        Screen.fullScreen = _isFullScreen;
        PlayerPrefs.SetInt("MasterFullscreen", _isFullScreen ? 1 : 0);
    }

}
