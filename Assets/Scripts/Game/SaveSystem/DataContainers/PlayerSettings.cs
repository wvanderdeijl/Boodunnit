using System;

public class PlayerSettings
{
    public int MusicVolume;
    public int SoundEffectVolume;
    public int TextSpeed;

    public static void ValidatePlayerSettings(PlayerSettings playerSettings)
    {
        if (playerSettings.MusicVolume < 0 || playerSettings.MusicVolume > 100)
        {
            playerSettings.MusicVolume = 100;
        }

        if (playerSettings.SoundEffectVolume < 0 || playerSettings.SoundEffectVolume > 100)
        {
            playerSettings.SoundEffectVolume = 100;
        }

        if (playerSettings.TextSpeed < 0 || playerSettings.TextSpeed > Enum.GetNames(typeof(TextSpeed)).Length)
        {
            playerSettings.TextSpeed = 0;
        }
    }
}
