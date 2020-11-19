using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class RatBehaviour : BaseMovement, IEntity, IPossessable
{
    private Canvas _staminaBarCanvas;

    public bool IsPossessed { get; set; }
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

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

    [SerializeField] private ClimbBehaviour _climbBehaviour;

    private void Awake()
    {
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 50f;
        _climbBehaviour.CurrentStamina = 50f;
        _climbBehaviour.Speed = 5f;

        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        _staminaBarCanvas = GameObject.Find("StaminaBarCanvas").GetComponent<Canvas>();
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

        if (_staminaBarCanvas)
        {
            _staminaBarCanvas.enabled = IsPossessed;
        }
    }

    public void EntityJump()
    {
        //Jump
        if (IsGrounded)
        {
            Jump();
        }
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
