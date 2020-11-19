using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class RatBehaviour : BaseMovement, IEntity, IPossessable
{
    [Header("Conversation Settings")]
    public bool RatCanTalkToBoolia;
    public CharacterList RatName;
    public Dialogue RatDialogue;
    public Question RatQuestion;
    public List<CharacterList> RatRelationships;

    [Header("Default Dialogue Answers")]
    public Sentence[] DefaultAnswersList;

    public bool CanTalkToBoolia
    {
        get { return RatCanTalkToBoolia; }
        set => RatCanTalkToBoolia = value;
    }
    public CharacterList CharacterName
    {
        get { return RatName; }
        set => RatName = value;
    }
    public Dialogue Dialogue
    {
        get { return RatDialogue; }
        set => RatDialogue = value;
    }
    public Question Question
    {
        get { return RatQuestion; }
        set => RatQuestion = value;
    }
    public List<CharacterList> Relationships
    {
        get { return RatRelationships; }
        set => RatRelationships = value;
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

    [SerializeField] private ClimbBehaviour _climbBehaviour;

    private void Awake()
    {
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 50f;
        _climbBehaviour.CurrentStamina = 50f;
        _climbBehaviour.Speed = 5f;

        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Rigidbody.angularVelocity = Vector3.zero;//ToDO: Tim what is this used for?

        if(!IsPossessed) MoveWithPathFinding();
    }

    public void Move(Vector3 direction)
    {
        if (_climbBehaviour.IsClimbing)
        {
            _climbBehaviour.Climb();
            return;
        }

        MoveEntityInDirection(direction);
    }

    public void DealFearDamage(float amount)
    {
        throw new NotImplementedException();
    }

    public IEnumerator CalmDown()
    {
        throw new NotImplementedException();
    }

    public void Faint()
    {
        throw new NotImplementedException();
    }

    public void CheckSurroundings()
    {
        throw new NotImplementedException();
    }

    public void UseFirstAbility()
    {
        _climbBehaviour.ToggleClimb();
    }
}
