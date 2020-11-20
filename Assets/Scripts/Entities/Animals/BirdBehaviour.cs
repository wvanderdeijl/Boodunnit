using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class BirdBehaviour : BaseMovement, IEntity, IPossessable
{
    [Header("Conversation Settings")]
    public bool BirdCanTalkToBoolia;
    public CharacterList BirdName;
    public Dialogue BirdDialogue;
    public Question BirdQuestion;
    public List<CharacterList> BirdRelationships;

    [Header("Default Dialogue Answers")]
    public Sentence[] DefaultAnswersList;

    public bool CanTalkToBoolia
    {
        get { return BirdCanTalkToBoolia; }
        set => BirdCanTalkToBoolia = value;
    }
    public CharacterList CharacterName
    {
        get { return BirdName; }
        set => BirdName = value;
    }
    public Dialogue Dialogue
    {
        get { return BirdDialogue; }
        set => BirdDialogue = value;
    }
    public Question Question
    {
        get { return BirdQuestion; }
        set => BirdQuestion = value;
    }
    public List<CharacterList> Relationships
    {
        get { return BirdRelationships; }
        set => BirdRelationships = value;
    }
    public Sentence[] DefaultAnswers
    {
        get { return DefaultAnswersList; }
        set => DefaultAnswersList = value;
    }

    public bool IsPossessed { get; set; }
    public Mesh NotGlidingMesh, GlidingMesh;
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

    [SerializeField] private GlideBehaviour _glideBehaviour;
    
    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
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

    public void Move(Vector3 direction)
    {
        if(_glideBehaviour.IsGliding) MoveEntityInDirection(direction, Speed / 1.5f);
        else MoveEntityInDirection(direction);
    }

    public void EntityJump()
    {
        //Jump
        if (IsGrounded)
        {
            Jump();
        }
    }

    public void CheckSurroundings()
    {
        throw new NotImplementedException();
    }

    public void UseFirstAbility()
    {
        _glideBehaviour.ToggleGlide();

        if (_glideBehaviour.IsGliding)
        {
            GetComponent<MeshFilter>().mesh = GlidingMesh;
        }
        else
        {
            GetComponent<MeshFilter>().mesh = NotGlidingMesh;
        }
    }
}
