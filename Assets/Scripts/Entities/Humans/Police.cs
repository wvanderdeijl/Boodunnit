
using Enums;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour, IHuman
{
    [Header("Conversation")]
    public Proffesion M_Proffesion;
    public string M_name;
    public Dialogue M_dialogue;
    public Question M_question;
    public Dialogue Dialogue { get { return M_dialogue; } }
    public Question Question { get { return M_question; } }
    public string Name { get { return M_name; } }
    public Proffesion Proffesion
    {
        get { return M_Proffesion; }
        set => M_Proffesion = value;
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
        throw new NotImplementedException();
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
