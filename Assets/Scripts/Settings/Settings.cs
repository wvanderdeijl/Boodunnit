using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Text SoundEffectValueText;
    public Text MusicValueText;

    public void OnValueChangedSoundEffectSlider(float volumeValue)
    {
        SoundEffectValueText.text = volumeValue.ToString();
    }

    public void OnValueChangedMusicSlider(float volumeValue)
    {
        MusicValueText.text = volumeValue.ToString();
    }

    public void OnValueChangedTextSpeedDropdown(int textSpeedIndex)
    {
        // ToDo change the conversation text speed
    }
    public void OnClickSaveChanges()
    {
        // ToDo save all changes made in the settings page
    }
}
