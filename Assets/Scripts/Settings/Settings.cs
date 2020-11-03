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

    public void OnClickSaveChanges()
    {
        _playerSettings.MusicVolume = (int) MusicSlider.value;
        _playerSettings.SoundEffectVolume = (int)SoundEffectSlider.value;
        _playerSettings.TextSpeed = TextSpeedDropdown.value;
        SaveHandler.Instance.SaveSettings(_playerSettings);
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
        if (SaveHandler.Instance.IsPlayerSettingsAvailable())
        {
            OnLoadPlayerSettings();
        } else
        {
            ChangeSlidersToDefaultValues();
        }
    }

    private void ChangeSlidersToDefaultValues()
    {
        SoundEffectValueText.text = "100";
        MusicValueText.text = "100";
        SoundEffectSlider.value = 100;
        MusicSlider.value = 100;
        TextSpeedDropdown.value = 0;
    }

    private void OnLoadPlayerSettings()
    {
        _playerSettings = SaveHandler.Instance.LoadSettings();
        SoundEffectValueText.text = _playerSettings.SoundEffectVolume.ToString();
        MusicValueText.text = _playerSettings.MusicVolume.ToString();
        SoundEffectSlider.value = _playerSettings.SoundEffectVolume;
        MusicSlider.value = _playerSettings.MusicVolume;
        TextSpeedDropdown.value = _playerSettings.TextSpeed;
    }
}