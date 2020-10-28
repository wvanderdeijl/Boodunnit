public class SaveHandler
{
    private static SaveHandler _instance;

    public static SaveHandler Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SaveHandler();
            }
            return _instance;
        }
    }

    public void SaveSettings(PlayerSettings playerSettings)
    {
        SerializationManager.Save("PlayerSettings", playerSettings);
    }

    public object LoadSettings()
    {
        return SerializationManager.Load<PlayerSettings>("PlayerSettings");
    }
}
