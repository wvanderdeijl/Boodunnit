using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class RatBehaviour : BaseMovement, IEntity, IPossessable
{
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
        NavMeshAgent.GetComponent<NavMeshAgent>();

        ChangePathFindingState(PathFindingState.Following);
        
        Target = GameObject.Find("Player");
    }

    private void Update()
    {
        Rigidbody.angularVelocity = Vector3.zero;

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
