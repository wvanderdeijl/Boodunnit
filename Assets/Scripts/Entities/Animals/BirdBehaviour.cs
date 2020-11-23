using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public class BirdBehaviour : BaseEntity, IPossessable
{
    public Mesh NotGlidingMesh, GlidingMesh;
    
    [SerializeField] private GlideBehaviour _glideBehaviour;
    
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        
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

    private void Update()
    {
        Rigidbody.isKinematic = !IsPossessed;
        if (!IsPossessed)  MoveWithPathFinding();
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_glideBehaviour.IsGliding) base.MoveEntityInDirection(direction, Speed / 1.5f);
        else base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        _glideBehaviour.ToggleGlide();
        GetComponent<MeshFilter>().mesh = _glideBehaviour.IsGliding ? GlidingMesh : NotGlidingMesh;
    }
}
