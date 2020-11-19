using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class Settings : MonoBehaviour
{
    public Text AudioValueText;
    private int _audioValue;
    public Text MusicValueText;
    private int _musicValue;
    public Text CameraSensivityValueText;
    private int _cameraSensivityValue;
    public Text ScreenResolutionValueText;
    private int _screenResolutionValueHeight;
    private int _screenResolutionValueWidth;
    public Text TextSpeedValueText;
    private int _textSpeedValue;

    private PlayerSettings _playerSettings;

    public void Awake()
    {
        SetDefaultValues();

        _playerSettings = new PlayerSettings();
        CheckIfPlayerSettingsExist();

        UpdateCanvasValues();
    }
    public void OnIncrementClicked()
    {

    }
    public void OnDecrementClicked()
    {

    }
    public void OnClickSaveChanges()
    {
        _playerSettings.AudioVolume = _audioValue;
        _playerSettings.MusicVolume = _musicValue;
        _playerSettings.CameraSensitivity = _cameraSensivityValue;
        _playerSettings.TextSpeed = 0;

        _playerSettings.ValidateData();

        SaveHandler.Instance.SaveDataContainer(_playerSettings);
    }

    private void SetDefaultValues()
    {
        _audioValue = 100;
        _musicValue = 100;
        _cameraSensivityValue = 1;
        _screenResolutionValueHeight = 1080;
        _screenResolutionValueWidth = 1920;
        _textSpeedValue = 0;
    }

    private void UpdateCanvasValues()
    {
        AudioValueText.text = _audioValue + "%";
        MusicValueText.text = _musicValue + "%";
        CameraSensivityValueText.text = _cameraSensivityValue.ToString();
        ScreenResolutionValueText.text = _screenResolutionValueWidth + " x " + _screenResolutionValueHeight;
        TextSpeedValueText.text = ((string) Enum.GetNames(typeof(TextSpeed)).GetValue(_textSpeedValue)).ToLower();
    }

    private void CheckIfPlayerSettingsExist()
    {
        PlayerSettings settings = SaveHandler.Instance.LoadDataContainer<PlayerSettings>();
        if (settings != null)
        {
            OnLoadPlayerSettings(settings);
        }
    }

    private void OnLoadPlayerSettings(PlayerSettings settings)
    {
        _audioValue = settings.AudioVolume;
        _musicValue = settings.MusicVolume;
        _cameraSensivityValue = settings.CameraSensitivity;
        _screenResolutionValueHeight = 1080;
        _screenResolutionValueWidth = 1920;
        _textSpeedValue = settings.TextSpeed;
    }
}