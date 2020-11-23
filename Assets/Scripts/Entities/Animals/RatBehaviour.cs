using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public class RatBehaviour : BaseEntity, IPossessable
{
    private ClimbBehaviour _climbBehaviour;
    private Canvas _staminaBarCanvas;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        _climbBehaviour = GetComponent<ClimbBehaviour>();
        _staminaBarCanvas = GameObject.Find("StaminaBarCanvas").GetComponent<Canvas>();
        
        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(BirdBehaviour)] = 5f,
            [typeof(VillagerBehaviour)] = 4f,
            [typeof(PoliceManBehaviour)] = 4f,
            [typeof(ILevitateable)] = 3f
        };
        
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 50f;
        _climbBehaviour.CurrentStamina = 50f;
        _climbBehaviour.Speed = 5f;
    }

    private void Update()
    {
        Rigidbody.isKinematic = !IsPossessed;
        if (!IsPossessed) MoveWithPathFinding();
        if (_staminaBarCanvas) _staminaBarCanvas.enabled = IsPossessed;
    }
    
    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_climbBehaviour.IsClimbing) _climbBehaviour.Climb();
        else base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        _climbBehaviour.ToggleClimb();
    }
}
