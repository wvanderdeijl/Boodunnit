using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;

public class BirdBehaviour : BaseEntity
{
    [SerializeField] private GlideBehaviour _glideBehaviour;
    
    private void Awake()
    {
        InitBaseEntity();
        _glideBehaviour = GetComponent<GlideBehaviour>();
        CanJump = true;
        
        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        IsScaredOfLevitatableObject = true;
        ScaredOfEntities = new Dictionary<CharacterType, float>()
        {
            [CharacterType.PoliceMan] = 3f,
            [CharacterType.Villager] = 3f
        };
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_glideBehaviour.IsGliding && !IsGrounded)
        {
            base.MoveEntityInDirection(direction, PossessionSpeed / 1.5f);
            PlayAudioOnMovement(1);
        }
        else
        {
            base.MoveEntityInDirection(direction);
            PlayAudioOnMovement(0);
        }
    }

    public override void UseFirstAbility()
    {
        _glideBehaviour.ToggleGlide();
    }
}
