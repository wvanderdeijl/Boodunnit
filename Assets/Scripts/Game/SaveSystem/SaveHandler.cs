using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class SaveHandler
{
    private static SaveHandler _instance;
    public static SaveHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveHandler();
            }

            return _instance;
        }
    }

    private readonly string _saveGame = "Save";
    private readonly string _playerSettings = "PlayerSettings";

    public void StartNewGame()
    {
        List<object> saveGameData = new List<object>();
        string saveGameDataString = JsonConvert.SerializeObject(saveGameData);
        PlayerPrefs.SetString(_saveGame, saveGameDataString);

        PlayerPrefs.Save();
    }

    public void DeleteSaveGame()
    {
        PlayerPrefs.DeleteKey(_saveGame);
        PlayerPrefs.Save();
    }

    public string LoadGame()
    {
        return PlayerPrefs.GetString(_saveGame);
    }

    public void UpdateSaveGame<T>(T dataToAddToSaveGame)
    {
        string saveGameString = PlayerPrefs.GetString(_saveGame);
        List<object> saveGameData = JsonConvert.DeserializeObject<List<object>>(saveGameString);

        saveGameData.Add(dataToAddToSaveGame);
        saveGameString = JsonConvert.SerializeObject(saveGameData);

        PlayerPrefs.SetString(_saveGame, saveGameString);
        PlayerPrefs.Save();
    }

    public void SaveSettings(string settings)
    {
        PlayerPrefs.SetString(_playerSettings, settings);
        PlayerPrefs.Save();
    }

    public string LoadSettings()
    {
        return PlayerPrefs.GetString(_playerSettings);
    }

    public bool IsPlayerSettingsAvailable()
    {
        return PlayerPrefs.GetString("PlayerSettings") != null ||
           PlayerPrefs.GetString("PlayerSettings") != "";
    }
}