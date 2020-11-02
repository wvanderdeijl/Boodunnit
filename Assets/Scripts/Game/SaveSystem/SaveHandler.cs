using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class. To access the Instance you type: SaveHandler.Instance.MethodYouWantToCall();
/// </summary>
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

    private readonly string _playerSettings = "PlayerSettings";

    /// <summary>
    /// This method will remove the current save game.
    /// </summary>
    public void DeleteSaveGame()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SaveGameProperty(string nameOfGameObject, string nameOfProperty, object propertyValue)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string unqiueKey = nameOfGameObject + "_" + nameOfProperty;

        string scene = PlayerPrefs.GetString(sceneName);

        if (scene != null && !scene.Equals(""))
        {
            Dictionary<string, object> propertiesInScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(scene);

            if (propertiesInScene.ContainsKey(unqiueKey))
            {
                propertiesInScene[unqiueKey] = propertyValue;
            } else
            {
                propertiesInScene.Add(unqiueKey, propertyValue);
            }

            PlayerPrefs.SetString(sceneName, JsonConvert.SerializeObject(propertiesInScene));
        } else
        {
            Dictionary<string, object> propertiesInScene = new Dictionary<string, object>();
            propertiesInScene.Add(unqiueKey, propertyValue);
            PlayerPrefs.SetString(sceneName, JsonConvert.SerializeObject(propertiesInScene));
        }

        PlayerPrefs.Save();
    }

    public T GetPropertyValueFromUniqueKey<T>(string nameOfGameObject, string nameOfProperty)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string uniqueKey = nameOfGameObject + "_" + nameOfProperty;

        Dictionary<string, object> propertiesInScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(PlayerPrefs.GetString(sceneName));

        if (propertiesInScene.ContainsKey(uniqueKey))
        {
            return (T)Convert.ChangeType(propertiesInScene[uniqueKey], typeof(T));
        }

        return default;
    }
    
    /// <summary>
    /// This method is used to save the player settings
    /// </summary>
    /// <param name="settings">JSON string with the serialized player settings</param>
    public void SaveSettings(string settings)
    {
        PlayerPrefs.SetString(_playerSettings, settings);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// This method is used to load the player settings.
    /// Since there are multiple values in the settings, I expect you to dezerialize it yourself
    /// using JsonConvert.DezerializeObject();
    /// </summary>
    /// <returns>JSON string with the player settings</returns>
    public string LoadSettings()
    {
        return PlayerPrefs.GetString(_playerSettings);
    }

    /// <summary>
    /// This method is used to check if there is a PlayerSettings playerprefs available.
    /// Should be called everytime the settings tab is accessed
    /// </summary>
    /// <returns>True or false depending if player settings is available</returns>
    public bool IsPlayerSettingsAvailable()
    {
        return PlayerPrefs.GetString("PlayerSettings") != null ||
           PlayerPrefs.GetString("PlayerSettings") != "";
    }
}