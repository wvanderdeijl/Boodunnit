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

    /// <summary>
    /// This method is used to save a property of a gameobject in a specific scene
    /// </summary>
    /// <param name="nameOfGameObject">Name of game object</param>
    /// <param name="nameOfProperty">Name of property you want to save</param>
    /// <param name="propertyValue">Value of the property</param>
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

    /// <summary>
    /// This method lets you get a value from a property of a scene
    /// </summary>
    /// <typeparam name="T">Type of value you return</typeparam>
    /// <param name="nameOfGameObject">Name of the game object</param>
    /// <param name="nameOfProperty">Name of the property</param>
    /// <param name="sceneName">Name of the scene, default it's null, so it'll get the current scene.  
    /// If you want to access properties from a different scene other then your current, pass the scene name.
    /// </param>
    /// <returns>Return the value from the unique key combined of nameOfGameObject and nameOfProperty</returns>
    public bool GetPropertyValueFromUniqueKey<T>(string nameOfGameObject, string nameOfProperty, out T propertyValue, string sceneName = null)
    {
        propertyValue = default;
        bool isValueFound = false;
        if(sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        string uniqueKey = nameOfGameObject + "_" + nameOfProperty;

        string sceneProperties = PlayerPrefs.GetString(sceneName);
        if (!String.IsNullOrEmpty(sceneProperties))
        {
            Dictionary<string, object> propertiesInScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(sceneProperties);
            if (propertiesInScene.ContainsKey(uniqueKey))
            {
                propertyValue = (T)Convert.ChangeType(propertiesInScene[uniqueKey], typeof(T));
                isValueFound = true;
            }
        }

        return isValueFound;
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