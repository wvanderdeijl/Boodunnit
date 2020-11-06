using System;
using System.Diagnostics;

public class PlayerSettings : BaseDataContainer
{
    public int MusicVolume;
    public int SoundEffectVolume;
    public int TextSpeed;

    public override void ValidateData()
    {
        if (MusicVolume < 0 || MusicVolume > 100)
        {
            MusicVolume = 100;
        }

        if (SoundEffectVolume < 0 || SoundEffectVolume > 100)
        {
            SoundEffectVolume = 100;
        }

        if (TextSpeed < 0 || TextSpeed > Enum.GetNames(typeof(TextSpeed)).Length)
        {
            TextSpeed = 0;
        }
    }
}
