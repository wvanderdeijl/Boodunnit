
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
    public NPCCharacter M_character;
    public Dialogue M_dialogue;
    public Question M_question;
    public Dialogue Dialogue { get { return M_dialogue; } }
    public Question Question { get { return M_question; } }
    public NPCCharacter Character
    {
        get { return M_character; }
        set => M_character = value;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

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
