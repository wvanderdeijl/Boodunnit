using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class Settings : MonoBehaviour
{
    public Text AudioValueText;
    public Text MusicValueText;
    public Text CameraSensivityValueText;
    public Text ScreenResolutionValueText;
    public Text TextSpeedValueText;

    public List<ScreenResolution> ScreenResolutions;

    private int _audioValue;
    private int _musicValue;
    private int _cameraSensivityValue;
    private int _screenResolutionValue;
    private int _textSpeedValue;

    private PlayerSettings _playerSettings;

    public void Awake()
    {
        SetDefaultValues();

        _playerSettings = new PlayerSettings();
        CheckIfPlayerSettingsExist();

        UpdateCanvasValues();
    }
    public void OnChangeAudio(int increment)
    {
        if (_audioValue + increment > 100)
        {
            _audioValue = 0;
        }
        else if (_audioValue + increment < 0)
        {
            _audioValue = 100;
        }
        else
        {
            _audioValue += increment;
        }
        AudioValueText.text = _audioValue + "%";
    }
    public void OnChangeMusic(int increment)
    {
        if (_musicValue + increment > 100)
        {
            _musicValue = 0;
        }
        else if (_musicValue + increment < 0)
        {
            _musicValue = 100;
        }
        else
        {
            _musicValue += increment;
        }
        MusicValueText.text = _musicValue + "%";
    }
    public void OnChangeCameraSensitivity(int increment)
    {
        if (_cameraSensivityValue + increment > 8)
        {
            _cameraSensivityValue = 1;
        }
        else if (_cameraSensivityValue + increment < 1)
        {
            _cameraSensivityValue = 8;
        }
        else
        {
            _cameraSensivityValue += increment;
        }
        CameraSensivityValueText.text = _cameraSensivityValue.ToString();
    }
    public void OnChangeScreenResolution(int increment)
    {
        if (_screenResolutionValue + increment > ScreenResolutions.Count - 1)
        {
            _screenResolutionValue = 0;
        }
        else if (_screenResolutionValue + increment < 0)
        {
            _screenResolutionValue = ScreenResolutions.Count - 1;
        }
        else
        {
            _screenResolutionValue += increment;
        }
        ScreenResolution screenResolution = ScreenResolutions[_screenResolutionValue];
        ScreenResolutionValueText.text = screenResolution.ScreenWidth + " x " + screenResolution.ScreenHeight;
    }
    public void OnChangeTextSpeed(int increment)
    {
        if (_textSpeedValue + increment > 2)
        {
            _textSpeedValue = 0;
        }
        else if (_textSpeedValue + increment < 0)
        {
            _textSpeedValue = 3;
        }
        else
        {
            _textSpeedValue += increment;
        }
        TextSpeedValueText.text = ((string)Enum.GetNames(typeof(TextSpeed)).GetValue(_textSpeedValue)).ToLower();
    }

    public void OnClickSaveChanges()
    {
        _playerSettings.AudioVolume = _audioValue;
        _playerSettings.MusicVolume = _musicValue;
        _playerSettings.CameraSensitivity = _cameraSensivityValue;
        _playerSettings.ScreenResolution = _screenResolutionValue;
        _playerSettings.TextSpeed = _textSpeedValue;

        _playerSettings.ValidateData();

        SaveHandler.Instance.SaveDataContainer(_playerSettings);
    }

    private void SetDefaultValues()
    {
        _audioValue = 100;
        _musicValue = 100;
        _cameraSensivityValue = 1;
        _screenResolutionValue = 0;
        _textSpeedValue = 0;
    }

    private void UpdateCanvasValues()
    {
        AudioValueText.text = _audioValue + "%";
        MusicValueText.text = _musicValue + "%";
        CameraSensivityValueText.text = _cameraSensivityValue.ToString();

        ScreenResolution screenResolution = ScreenResolutions[_screenResolutionValue];
        ScreenResolutionValueText.text = screenResolution.ScreenWidth + " x " + screenResolution.ScreenHeight;

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
        _screenResolutionValue = settings.ScreenResolution;
        _textSpeedValue = settings.TextSpeed;
    }
}