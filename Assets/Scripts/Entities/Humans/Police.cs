
using Enums;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : BaseMovement, IHuman, IPossessable
{
    private Transform _cameraTransform;

    [Header("Conversation")]
    public CharacterList Name;
    public Dialogue dialogue;
    public Question question;
    public List<CharacterList> relationships;

    [Header("Default Answers")]
    public Sentence[] DefaultAnswersList;

    public Dialogue Dialogue { get { return dialogue; } }
    public Question Question { get { return question; } }

    public List<CharacterList> Relationships { get { return relationships; } }

    public Sentence[] DefaultAnswers
    {
        get { return DefaultAnswersList; }
        set => DefaultAnswersList = value;
    }
    public CharacterList CharacterName
    {
        get { return Name; }
        set => Name = value;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    public bool IsPossessed { get; set; }
    public float FearThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FearDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FaintDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public EmotionalState EmotionalState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Dictionary<Type, float> ScaredOfGameObjects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IEnumerator CalmDown()
    {
        throw new NotImplementedException();
    }

    public void Move(Vector3 direction)
    {
        MoveEntityInDirection(direction);
    }

    public void CheckSurroundings()
    {
        throw new NotImplementedException();
    }

    public void DealFearDamage(float amount)
    {
        throw new NotImplementedException();
    }

    public void Faint()
    {
        throw new NotImplementedException();
    }

    public void UseFirstAbility()
    {
        throw new NotImplementedException();
    }

    public void UseSecondAbility()
    {
        throw new NotImplementedException();
    }

    public IEntity GetBehaviour()
    {
        return this;
    }
}
