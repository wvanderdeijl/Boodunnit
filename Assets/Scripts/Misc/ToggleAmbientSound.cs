using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAmbientSound : MonoBehaviour
{
    public List<AudioClip> clips;

    public int AmbientSoundPlaying = 0;

    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour player = other.gameObject.GetComponent<PlayerBehaviour>();
        BaseEntity entity = other.gameObject.GetComponent<BaseEntity>();
        if (player || entity.IsPossessed)
        {
            if (!other.isTrigger)
            {
                if (AmbientSoundPlaying == 0)
                {
                    FindObjectOfType<SoundManager>().StopSound("CrimeScene_Sea");
                    FindObjectOfType<SoundManager>().PlaySound("CrimeScene_Town");
                    AmbientSoundPlaying = 1;
                }
                else if (AmbientSoundPlaying == 1)
                {
                    FindObjectOfType<SoundManager>().StopSound("CrimeScene_Town");
                    FindObjectOfType<SoundManager>().PlaySound("CrimeScene_Sea");
                    AmbientSoundPlaying = 0;
                }
            }
        }
    }
}
