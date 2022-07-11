using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class UIMenu : MonoBehaviour
{
    CameraMovement vCam;

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject pauseMenuHolder;
    public GameObject controlsMenuHolder;
    public GameObject canvas1, canvas2;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public int[] screenWidths;
    public bool play = false;

    int activeScreenResIndex;

    void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for(int i=0; i<resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }

        fullscreenToggle.isOn = isFullscreen;
        canvas1.SetActive(false);
    }

    public void Play()
    {
        play = true;
        vCam = FindObjectOfType<CameraMovement>();
        vCam.StopRotation();
        vCam.transform.rotation = Quaternion.Euler(45, 15, 0);
        canvas1.SetActive(true);
        canvas2.SetActive(true);
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        optionsMenuHolder.transform.GetChild(0).gameObject.SetActive(false);
        optionsMenuHolder.transform.GetChild(1).gameObject.SetActive(true);
        controlsMenuHolder.SetActive(false);
        controlsMenuHolder.transform.GetChild(0).gameObject.SetActive(false);
        controlsMenuHolder.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(!mainMenuHolder.activeSelf);
    }

    public void OptionsMenu()
    {
        optionsMenuHolder.SetActive(!optionsMenuHolder.activeSelf);
    }

    public void PauseMenu()
    {
        pauseMenuHolder.SetActive(!pauseMenuHolder.activeSelf);
    }

    public void ControlsMenu()
    {
        controlsMenuHolder.SetActive(!controlsMenuHolder.activeSelf);
    }

    public void SetScreenResolution(int i)
    {
        if(resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16/9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i]/aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        for(int i=0; i<resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if(isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void TestSfxVolume()
    {
        AudioManager.instance.PlaySound("Shoot", transform.position);
    }

    public void StartNewGame()
    {
        Destroy(AudioManager.instance);
        SceneManager.LoadScene("SampleScene");
    }
}
