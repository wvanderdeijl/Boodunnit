using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
        List<Dictionary<string, object>> saveGameData = new List<Dictionary<string, object>>();
        string saveGameDataString = JsonConvert.SerializeObject(saveGameData);
        PlayerPrefs.SetString(_saveGame, saveGameDataString);

        PlayerPrefs.Save();
    }

    public void DeleteSaveGame()
    {
        PlayerPrefs.DeleteKey(_saveGame);
        PlayerPrefs.Save();
    }

    public List<Dictionary<string, object>> LoadGame()
    {
        return DezerializeSaveGameList();
    }

    public Dictionary<string, object> LoadGame(string sceneName)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        return GetSceneFromSaveGameList(saveGame, sceneName);
    }

    public Dictionary<string, object> LoadGameObjects(string sceneName)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        Dictionary<string, object> scene = GetSceneFromSaveGameList(saveGame, sceneName);
        Dictionary<string, object> objects = JsonConvert.DeserializeObject<Dictionary<string, object>>(scene[sceneName].ToString());

        return objects;
    }

    /**
     * sceneName : string -> I expect you to use Unity to get name of current scene.
     * nameOfObject : string -> Name of the object / gameobject you want data to be saved.
     *                          I want you to use very specific names for each gameobject.
     *                          If it's a gameobject, use Unity way to get name of gameobject
     * 
     * dataToSave : object -> Data you want to save.
     **/
    public void UpdateSaveGame(string sceneName, string nameOfObject, object dataToSave)
    {
        List<Dictionary<string, object>> saveGame = DezerializeSaveGameList();
        /**
         * Hoofd lijst:
         *  - Dictionary<Scene 1, object to Save>
         *          - Dictionary<door, true>
         *          - Dictionary<Politieman, Dictionary<string, object>
         *  - Dictionary<Scene 2, objects to save>
         *  
         *  Dict bestaat
         *      - Haal dictionary van scene op
         *      - Voeg nieuwe data toe.
         *      
         *   ----------------------------------
         *   
         *   - Scene 2
         *      - Politieman
         *          - Positie -> x,y,z
         *          - hasKey -> false
         *   
         *   ------------------------------------
         *   1. Bestaat de scene? Ja: kijk op object bestaat: Ja? update.
         *   2. Bestaat de scene? Nee: Dan kan het object ook niet bestaat dus voeg toe.
         **/

        bool hasScene = saveGame.Any(scene => scene.ContainsKey(sceneName));
        if (hasScene)
        {
            Dictionary<string, object> scene = new Dictionary<string, object>();

            // Refector later to be more effiecient
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