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
        InitBaseEntity();
        _climbBehaviour = GetComponent<ClimbBehaviour>();
        CanJump = true;

        FearThreshold = 50;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        IsScaredOfLevitatableObject = true;
        ScaredOfEntities = new Dictionary<CharacterType, float>()
        {
            [CharacterType.Cat] = 5f,
            [CharacterType.Bird] = 5f,
        };
        
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 50f;
        _climbBehaviour.CurrentStamina = 50f;
        _climbBehaviour.Speed = 5f;
    }
    
    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_climbBehaviour.IsClimbing)
        {
            PlayAudioOnMovement(1);
            _climbBehaviour.Climb();
        }
        else
        {
            PlayAudioOnMovement(0);
            base.MoveEntityInDirection(direction);
        }
    }

    public override void UseFirstAbility()
    {
        _climbBehaviour.ToggleClimb();
        base.UseFirstAbility();
    }
}
