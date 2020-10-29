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

    private readonly string _musicVolume = "Music";
    private readonly string _soundEffectVolume = "SoundEffect";
    private readonly string _textSpeed = "TextSpeed";

    private void Awake()
    {
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
        Dictionary<string, string> playerSettings = new Dictionary<string, string>();
        playerSettings.Add(_musicVolume, MusicSlider.value.ToString());
        playerSettings.Add(_soundEffectVolume, SoundEffectSlider.value.ToString());
        playerSettings.Add(_textSpeed, TextSpeedDropdown.value.ToString());

        string playerSettingsString = JsonConvert.SerializeObject(playerSettings);
        SaveHandler.Instance.SaveSettings(playerSettingsString);
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
        string playerSettingsString = SaveHandler.Instance.LoadSettings();
        Dictionary<string, string> playerSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(playerSettingsString);

        SoundEffectValueText.text = playerSettings[_soundEffectVolume];
        MusicValueText.text = playerSettings[_musicVolume];
        SoundEffectSlider.value = int.Parse(playerSettings[_soundEffectVolume]);
        MusicSlider.value = int.Parse(playerSettings[_musicVolume]);
        TextSpeedDropdown.value = int.Parse(playerSettings[_textSpeed]);
    }
}