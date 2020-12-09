using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;

public class BirdBehaviour : BaseEntity
{
    public Mesh NotGlidingMesh, GlidingMesh;
    
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
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(PoliceManBehaviour)] = 3f,
            [typeof(VillagerBehaviour)] = 3f,
            [typeof(ILevitateable)] = 3f
        };
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_glideBehaviour.IsGliding && !IsGrounded) base.MoveEntityInDirection(direction, PossessionSpeed / 1.5f);
        else base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        _glideBehaviour.ToggleGlide();
        GetComponent<MeshFilter>().mesh = _glideBehaviour.IsGliding ? GlidingMesh : NotGlidingMesh;
    }
}
