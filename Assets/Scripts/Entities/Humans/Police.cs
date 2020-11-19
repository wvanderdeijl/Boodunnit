
using Enums;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : BaseMovement, IEntity, IPossessable
{
    private Transform _cameraTransform;

    [Header("Conversation Settings")]
    public bool PoliceCanTalkToBoolia;
    public CharacterList PoliceName;
    public Dialogue PoliceDialogue;
    public Question PoliceQuestion;
    public List<CharacterList> PoliceRelationships;

    [Header("Default Dialogue Answers")]
    public Sentence[] DefaultAnswersList;

    public bool CanTalkToBoolia
    {
        get { return PoliceCanTalkToBoolia; }
        set => PoliceCanTalkToBoolia = value;
    }
    public CharacterList CharacterName
    {
        get { return PoliceName; }
        set => PoliceName = value;
    }
    public Dialogue Dialogue
    {
        get { return PoliceDialogue; }
        set => PoliceDialogue = value;
    }
    public Question Question
    {
        get { return PoliceQuestion; }
        set => PoliceQuestion = value;
    }
    public List<CharacterList> Relationships
    {
        get { return PoliceRelationships; }
        set => PoliceRelationships = value;
    }
    public Sentence[] DefaultAnswers
    {
        get { return DefaultAnswersList; }
        set => DefaultAnswersList = value;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool IsPossessed { get; set; }
    public float FearThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FearDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FaintDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public EmotionalState EmotionalState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Dictionary<Type, float> ScaredOfGameObjects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IEnumerator CalmDown()
    {
        yield return null;
    }

    private void Update()
    {
        if (ConversationManager.hasConversationStarted)
        {
            return;
        }

        if (!IsPossessed)
        {
            Rigidbody.isKinematic = true;
            MoveWithPathFinding();
        }
        else
        {
            Rigidbody.isKinematic = false;
        }
    }

    public void Move(Vector3 direction)
    {
        if (ConversationManager.hasConversationStarted)
        {
            return;
        }

        MoveEntityInDirection(direction);
    }

    public void EntityJump()
    {

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

    public void UseFirstAbility()
    {

    }

    public void UseSecondAbility()
    {

    }
}
