using System.Collections;
using Entities;
using UnityEngine;

public class SirBonkelBehaviour : BaseEntity
{
    public float FadeDuration = 1f;

    private Transform _newSpawnTransform;

    public void SpawnToNewLocation(Transform newTransform)
    {
        _newSpawnTransform = newTransform;

        if (transform.position != _newSpawnTransform.position)
        {
            StartCoroutine("FadeInAndOut");
        }
    }

    private IEnumerator FadeInAndOut()
    {
        MeshRenderer meshRend = GetComponent<MeshRenderer>();
        Material[] materials = meshRend.materials;

        // Fade out
        float currentTime = 0;
        while (currentTime < FadeDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
            ChangeColorOpacity(materials, 1 - currentTime / FadeDuration);
        }

        if (_newSpawnTransform)
        {
            transform.position = _newSpawnTransform.position;
            transform.rotation = _newSpawnTransform.rotation;
        }

        // Fade in
        currentTime = 0;
        while (currentTime < FadeDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
            ChangeColorOpacity(materials, currentTime / FadeDuration);
        }
    }

    private void ChangeColorOpacity(Material[] materials, float opacity)
    {
        foreach (Material m in materials)
        {
            Color newColor = m.color;
            newColor.a = opacity;
            m.color = newColor;
        }
    }
    
    public override void UseFirstAbility()
    {
        //TODO sir Bonkel ability? WHEEZE
    }
}
