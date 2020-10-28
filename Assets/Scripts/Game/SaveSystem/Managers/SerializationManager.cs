using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager 
{

    private static string _locationToSaveData = "/data/";

    public static bool Save(string nameOfThingToSave, object dataToSave)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + _locationToSaveData))
        {
            Directory.CreateDirectory(Application.persistentDataPath + _locationToSaveData);
        }

        string pathName = Application.persistentDataPath + _locationToSaveData + nameOfThingToSave + ".save";
        using (FileStream file = File.Create(pathName))
        {
            formatter.Serialize(file, dataToSave);
            file.Close();
        }

        return true;
    }

    public static T Load<T>(string thingToLoad)
    {
        string pathToLoad = (Application.persistentDataPath + _locationToSaveData + thingToLoad + ".save");
        if (!File.Exists(pathToLoad))
        {
            return default;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        using (FileStream file = File.Open(pathToLoad, FileMode.Open))
        {
            try
            {
                object dataToLoad = formatter.Deserialize(file);
                file.Close();
                return (T) dataToLoad;
            }
            catch
            {
                Debug.LogErrorFormat("Failed to load file at {0}", thingToLoad);
                file.Close();
                return default;
            }
        }    
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        return new BinaryFormatter();
    }
}
