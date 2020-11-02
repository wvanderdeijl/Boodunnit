using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton class. To access the Instance you type: SaveHandler.Instance.MethodYouWantToCall();
/// 
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

    private readonly string _saveGame = "Save";
    private readonly string _playerSettings = "PlayerSettings";

    /// <summary>
    /// This method is used to start a new game. It'll create a new list that contains the scene and object data.
    /// IMPORTANT: You'll probably want to call DeleteSaveGame() first to remove the previous save.
    /// </summary>
    public void StartNewGame()
    {
        List<Dictionary<string, object>> saveGameData = new List<Dictionary<string, object>>();
        string saveGameDataString = JsonConvert.SerializeObject(saveGameData);
        PlayerPrefs.SetString(_saveGame, saveGameDataString);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// This method will remove the current save game.
    /// </summary>
    public void DeleteSaveGame()
    {
        PlayerPrefs.DeleteKey(_saveGame);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// This method return the save game list that contains all scenes
    /// IMPORTANT: Only call this method when you really need the entire save game list.
    /// </summary>
    /// <returns>List with a dictionary per scene.</returns>
    public List<Dictionary<string, object>> LoadGame()
    {
        return DezerializeSaveGameList();
    }

    /// <summary>
    /// This method returns the dictionary from a specific scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene. Please use the unity method to get the scene name to avoid any problems.</param>
    /// <returns>Dictionary of the scene</returns>
    public Dictionary<string, object> LoadGame(string sceneName)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        return GetSceneFromSaveGameList(saveGame, sceneName);
    }

    /// <summary>
    /// This method returns the objects in the scene directory.
    /// </summary>
    /// <param name="sceneName">Name of the scene. Please use the unity method to get the scene name to avoid any problems.</param>
    /// <returns>Dictionary of the objects within a specific scene.</returns>
    public Dictionary<string, object> LoadGameObjects(string sceneName)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        Dictionary<string, object> scene = GetSceneFromSaveGameList(saveGame, sceneName);
        Dictionary<string, object> objectsInScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(scene[sceneName].ToString());

        return objectsInScene;
    }

    /// <summary>
    /// This method loads a specific object value in a specific scene.
    /// </summary>
    /// <typeparam name="T">You know how you saved it, if you want to load it, give the type of object</typeparam>
    /// <param name="sceneName">Name of the scene. Please use the unity method to get the scene name to avoid any problems.</param>
    /// <param name="nameOfObject">Name of the object you want to load.</param>
    /// <returns>Value of given type</returns>
    public T LoadGameObject<T>(string sceneName, string nameOfObject)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        Dictionary<string, object> scene = GetSceneFromSaveGameList(saveGame, sceneName);
        Dictionary<string, object> objectsInScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(scene[sceneName].ToString());

        return objectsInScene.ContainsKey(nameOfObject) ? JsonConvert.DeserializeObject<T>(objectsInScene[nameOfObject].ToString()) : default;
    }

    /// <summary>
    /// This method is used to save the game. For now it's used as an "Autosave"'
    /// Call this method everytime the player did something that needs to be remembered.
    /// </summary>
    /// <param name="sceneName">Name of the scene. Please use the unity method to get the scene name to avoid any problems.</param>
    /// <param name="nameOfObject">Name of the object you want to save.</param>
    /// <param name="dataToSave">Data you want to save</param>
    public void UpdateSaveGame(string sceneName, string nameOfObject, object dataToSave)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
       
        bool hasScene = saveGame.Any(scene => scene.ContainsKey(sceneName));
        if (hasScene)
        {
            Dictionary<string, object> scene = new Dictionary<string, object>();

            foreach(Dictionary<string, object> sceneInList in saveGame)
            {
                if (sceneInList.ContainsKey(sceneName))
                {
                    scene = sceneInList;
                }
            }

            bool isObjectInDictionary = scene.Any(objectInDictionary => objectInDictionary.Key == nameOfObject);
            if (isObjectInDictionary)
            {
                scene[nameOfObject] = dataToSave;
            } else
            {
                scene.Add(nameOfObject, dataToSave);
            }

            return;
        }

        Dictionary<string, object> SceneDictionary = new Dictionary<string, object>();
        Dictionary<string, object> saveData = new Dictionary<string, object>();

        saveData.Add(nameOfObject, dataToSave);
        SceneDictionary.Add(sceneName, saveData);

        saveGame.Add(SceneDictionary);

        string saveGameString = JsonConvert.SerializeObject(saveGame);
        PlayerPrefs.SetString(_saveGame, saveGameString);
        PlayerPrefs.Save();
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

    /// <summary>
    /// This method is used when you still need to dezerialize data.
    /// This is because, if you have a complex object, not everything will be dezerialized right away.
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="dataToDezerialize">JSON string of the data you want to dezerialize</param>
    /// <returns>Object with given type.</returns>
    public T DezerializeGameData<T>(string dataToDezerialize)
    {
        T dezerializedData = JsonConvert.DeserializeObject<T>(dataToDezerialize);
        return dezerializedData != null ? dezerializedData : default;
    }

    private List<Dictionary<string, object>> DezerializeSaveGameList()
    {
        string saveGameString = PlayerPrefs.GetString(_saveGame);
        List<Dictionary<string, object>> saveGame = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(saveGameString);
        return saveGame;
    }

    private Dictionary<string, object> GetSceneFromSaveGameList(List<Dictionary<string, object>> saveGame, string sceneName)
    {
        foreach (Dictionary<string, object> sceneInList in saveGame)
        {
            if (sceneInList.ContainsKey(sceneName))
            {
                return sceneInList;
            }
        }

        return null;
    }
}