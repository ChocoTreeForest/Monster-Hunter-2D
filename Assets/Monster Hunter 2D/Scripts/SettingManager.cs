using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Text bgmVolumeText;
    public Text sfxVolumeText;
    public Dropdown resolutionDropdown;
    public Toggle fullScreen;
    public Toggle window;

    private Resolution[] availableResolutions;
    private Resolution currentResolution;
    private int selectedResolutionIndex;
    private int previousResolutionIndex;

    void Start()
    {
        LoadResolutionSettings();
        InitResolutionDropdown();
        ApplyLoadedResolution();

        LoadScreenModeSetting();
        ApplyScreenMode();

        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        ApplyBGMVolume(bgmSlider.value);
        ApplySFXVolume(sfxSlider.value);

        UpdateVolumeText();
    }

    public void OpenSetting()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        settingPanel.SetActive(true);

        previousResolutionIndex = selectedResolutionIndex; //현재 해상도 저장
    }

    public void CloseSettings()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        selectedResolutionIndex = previousResolutionIndex;
        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        settingPanel.SetActive(false);
    }

    public void OnBGMSliderChanged()
    {
        ApplyBGMVolume(bgmSlider.value);
    }

    public void OnSFXSliderChanged()
    {
        ApplySFXVolume(sfxSlider.value);
    }

    public void ChangeBGMVolume(float delta)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
        bgmSlider.value = Mathf.Clamp(bgmSlider.value + delta, 0f, 1f);
        ApplyBGMVolume(bgmSlider.value);
        UpdateVolumeText();
    }

    public void ChangeSFXVolume(float delta)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
        sfxSlider.value = Mathf.Clamp(sfxSlider.value + delta, 0f, 1f);
        ApplySFXVolume(sfxSlider.value);
        UpdateVolumeText();
    }

    private void ApplyBGMVolume(float volume)
    {
        AudioManager.instance.bgmVolume = volume;
        foreach (var player in AudioManager.instance.bgmPlayers)
        {
            player.volume = volume;
        }
        PlayerPrefs.SetFloat("BGMVolume", volume);
        UpdateVolumeText();
        PlayerPrefs.Save();
    }
    
    private void ApplySFXVolume(float volume)
    {
        AudioManager.instance.sfxVolume = volume;
        foreach (var player in AudioManager.instance.sfxPlayers)
        {
            player.volume = volume;
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        UpdateVolumeText();
        PlayerPrefs.Save();
    }

    private void UpdateVolumeText()
    {
        bgmVolumeText.text = $"볼륨: {(bgmSlider.value * 100f):0}";
        sfxVolumeText.text = $"볼륨: {(sfxSlider.value * 100f):0}";
    }

    private void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        availableResolutions = Screen.resolutions; //Screen.resolutions API를 사용해 지원되는 해상도 목록을 가져옴

        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            var res = availableResolutions[i];
            resolutionOptions.Add($"{res.width} x {res.height}");
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnResolutionChanged(int index)
    {
        selectedResolutionIndex = index;
    }

    public void ApplySettings()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        var selectedResolution = availableResolutions[selectedResolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        previousResolutionIndex = selectedResolutionIndex;

        PlayerPrefs.SetInt("ResolutionIndex", selectedResolutionIndex);
        PlayerPrefs.Save();
    }

    private void LoadResolutionSettings()
    {
        availableResolutions = Screen.resolutions;

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            selectedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            // 인덱스가 유효한지 확인
            if (selectedResolutionIndex < 0 || selectedResolutionIndex >= availableResolutions.Length)
            {
                selectedResolutionIndex = availableResolutions.Length - 1;
            }
        }
        else
        {
            selectedResolutionIndex = availableResolutions.Length - 1;
            PlayerPrefs.SetInt("ResolutionIndex", selectedResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    private void ApplyLoadedResolution()
    {
        var selectedResolution = availableResolutions[selectedResolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    public void ChangeScreenMode()
    {
        if (fullScreen.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // 전체화면
            SaveScreenMode(0);
        }
        else if (window.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; // 창모드
            SaveScreenMode(1);
        }
    }

    private void SaveScreenMode(int mode)
    {
        PlayerPrefs.SetInt("ScreenMode", mode);
        PlayerPrefs.Save();
    }

    private void LoadScreenModeSetting()
    {
        int currentMode = PlayerPrefs.GetInt("ScreenMode", 0);
        switch (currentMode)
        {
            case 0:
                fullScreen.isOn = true;
                window.isOn = false;
                break;
            case 1:
                fullScreen.isOn = false;
                window.isOn = true;
                break;
            default:
                fullScreen.isOn = true;
                window.isOn = false;
                break;
        }
    }

    private void ApplyScreenMode()
    {
        if (fullScreen.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (window.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
