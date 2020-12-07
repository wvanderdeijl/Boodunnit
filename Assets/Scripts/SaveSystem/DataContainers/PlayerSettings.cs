using System;
using System.Diagnostics;

public class PlayerSettings : BaseDataContainer
{
    public int MusicVolume;
    public int AudioVolume;
    public int CameraSensitivity;
    public int ScreenResolution;
    public int TextSpeed;
    public bool IsFullscreen;

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

        if (ScreenResolution < 0 || ScreenResolution > 15)
        {
            ScreenResolution = 0;
        }

        if (TextSpeed < 0 || TextSpeed > Enum.GetNames(typeof(TextSpeed)).Length)
        {
            TextSpeed = 0;
        }
    }
}
