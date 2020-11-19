using System;
using System.Diagnostics;

public class PlayerSettings : BaseDataContainer
{
    public int MusicVolume;
    public int AudioVolume;
    public int CameraSensitivity;
    public int TextSpeed;

    public override void ValidateData()
    {
        if (MusicVolume < 0 || MusicVolume > 100)
        {
            MusicVolume = 100;
        }

        if (AudioVolume < 0 || AudioVolume > 100)
        {
            AudioVolume = 100;
        }

        if (CameraSensitivity < 1 || CameraSensitivity > 8)
        {
            CameraSensitivity = 1;
        }

        if (TextSpeed < 0 || TextSpeed > Enum.GetNames(typeof(TextSpeed)).Length)
        {
            TextSpeed = 0;
        }
    }
}
