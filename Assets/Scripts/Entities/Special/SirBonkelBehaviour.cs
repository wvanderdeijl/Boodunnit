using Enums;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirBonkelBehaviour : MonoBehaviour, IEntity
{
    [Header("Conversation Settings")]
    public bool SirBonkelCanTalkToBoolia;
    public CharacterList SirBonkelName;
    public Dialogue SirBonkelDialogue;
    public Question SirBonkelQuestion;
    public List<CharacterList> SirBonkelRelationships;

    [Header("Default Dialogue Answers")]
    public Sentence[] DefaultAnswersList;

    public bool CanTalkToBoolia
    {
        get { return SirBonkelCanTalkToBoolia; }
        set => SirBonkelCanTalkToBoolia = value;
    }
    public CharacterList CharacterName
    {
        get { return SirBonkelName; }
        set => SirBonkelName = value;
    }
    public Dialogue Dialogue
    {
        get { return SirBonkelDialogue; }
        set => SirBonkelDialogue = value;
    }
    public Question Question
    {
        get { return SirBonkelQuestion; }
        set => SirBonkelQuestion = value;
    }
    public List<CharacterList> Relationships
    {
        get { return SirBonkelRelationships; }
        set => SirBonkelRelationships = value;
    }
    public Sentence[] DefaultAnswers
    {
        get { return DefaultAnswersList; }
        set => DefaultAnswersList = value;
    }

    public bool IsPossessed { get; set; }
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

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

    public IEnumerator CalmDown()
    {
        yield return null;
    }

    public void CheckSurroundings()
    {
    }

    public void DealFearDamage(float amount)
    {
    }

    public void Faint()
    {
    }

    public void Move(Vector3 direction)
    {
    }

    public void UseFirstAbility()
    {
    }
}
