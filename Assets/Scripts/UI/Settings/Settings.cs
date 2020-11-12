using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class Settings : MonoBehaviour
{
    public Text SoundEffectValueText;
    public Slider SoundEffectSlider;
    public Text MusicValueText;
    public Slider MusicSlider;
    public Text SensivityValueText;
    public Slider SensitivitySlider;

    public Dropdown TextSpeedDropdown;

    private PlayerSettings _playerSettings;

    private void Awake()
    {
        _playerSettings = new PlayerSettings();
        AddOptionsToDropdown();
        CheckIfPlayerSettingsExist();
    }

    public void OnValueChangedSoundEffectSlider(float volumeValue)
    {
        SoundEffectValueText.text = volumeValue.ToString();
    }

    public void OnValueChangedMusicSlider(float volumeValue)
    {
        MusicValueText.text = volumeValue.ToString();
    }

    public void OnValueChangedSensitivitySlider(float volumeValue)
    {
        SensivityValueText.text = volumeValue.ToString();
    }

    public void OnClickSaveChanges()
    {
        _playerSettings.MusicVolume = (int) MusicSlider.value;
        _playerSettings.SoundEffectVolume = (int) SoundEffectSlider.value;
        _playerSettings.CameraSensitivity = (int) (SensitivitySlider.value / 12.5f);
        _playerSettings.TextSpeed = TextSpeedDropdown.value;

        _playerSettings.ValidateData();

        SaveHandler.Instance.SaveDataContainer(_playerSettings);
    }

    private void AddOptionsToDropdown()
    {
        List<string> textSpeedOptions = new List<string>();
        foreach(TextSpeed textSpeed in Enum.GetValues(typeof(TextSpeed)))
        {
            textSpeedOptions.Add(textSpeed.ToString());
        }

        TextSpeedDropdown.AddOptions(textSpeedOptions);
    }

    private void CheckIfPlayerSettingsExist()
    {
        PlayerSettings settings = SaveHandler.Instance.LoadDataContainer<PlayerSettings>();
        if (settings != null)
        {
            OnLoadPlayerSettings(settings);
        } else
        {
            ChangeSlidersToDefaultValues();
        }
    }

    private void ChangeSlidersToDefaultValues()
    {
        SoundEffectValueText.text = "100";
        MusicValueText.text = "100";
        SensivityValueText.text = "1";
        SoundEffectSlider.value = 100;
        MusicSlider.value = 100;
        SensitivitySlider.value = 1;
        TextSpeedDropdown.value = 0;
    }

    private void OnLoadPlayerSettings(PlayerSettings settings)
    {
        SoundEffectValueText.text = settings.SoundEffectVolume.ToString();
        MusicValueText.text = settings.MusicVolume.ToString();
        SensivityValueText.text = (settings.CameraSensitivity * 12.5f).ToString();
        SoundEffectSlider.value = settings.SoundEffectVolume;
        MusicSlider.value = settings.MusicVolume;
        SensitivitySlider.value = settings.CameraSensitivity * 12.5f;
        TextSpeedDropdown.value = settings.TextSpeed;
    }
}