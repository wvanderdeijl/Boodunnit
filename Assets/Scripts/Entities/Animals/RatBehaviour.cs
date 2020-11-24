using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;

public class RatBehaviour : BaseEntity
{
    private ClimbBehaviour _climbBehaviour;

    private void Awake()
    {
        _climbBehaviour = GetComponent<ClimbBehaviour>();

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
        if (_climbBehaviour.StaminaBarCanvas) _climbBehaviour.StaminaBarCanvas.enabled = IsPossessed;
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
