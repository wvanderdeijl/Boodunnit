using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAmbientSound : MonoBehaviour
{
    public List<AudioClip> clips;

    [HideInInspector]
    public int AmbientSoundPlaying = 0;

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();
        PossessionBehaviour possession = player.PossessionBehaviour;
        if (player == other.GetComponent<PlayerBehaviour>() || possession.TargetBehaviour)
        {
            if (!other.isTrigger)
            {
                if (AmbientSoundPlaying == 0)
                {
                    SoundManager.Instance.StopSound("CrimeScene_Sea");
                    SoundManager.Instance.PlaySound("CrimeScene_Town");
                    AmbientSoundPlaying = 1;
                }
                else if (AmbientSoundPlaying == 1)
                {
                    SoundManager.Instance.StopSound("CrimeScene_Town");
                    SoundManager.Instance.PlaySound("CrimeScene_Sea");
                    AmbientSoundPlaying = 0;
                }
            }
        }
    }
}
