using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip AudioClip;

    [HideInInspector]
    public float Volume;

    public bool ShouldLoop;

    public bool ShouldStartOnStart;

    public bool IsMusicVolume;

    [HideInInspector]
    public AudioSource AudioSource;
}